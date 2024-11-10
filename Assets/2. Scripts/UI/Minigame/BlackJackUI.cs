using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public enum CardShape { Spade, Diamond, Heart, Clover }
public enum GameResult { Player_BlackJack, Player_Win, Dealer_BlackJack, Dealer_Win, Draw }

public struct Card
{
    public CardShape shape;
    public int number;

    public Card(CardShape shape, int number)
    {
        this.shape = shape;
        this.number = number;
    }
}

[System.Serializable]
public class MsgPanel
{
    public GameObject panel;
    public TextMeshProUGUI msgText;

    public void OnMessage(string msg, float sec)
    {
        panel.SetActive(true);

        Image[] images = panel.GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.color = Color.white;
            image.DOFade(0.0f, sec);
        }

        msgText.text = msg;
        msgText.color = Color.white;
        msgText.DOFade(0.0f, sec).OnComplete(() => { panel.SetActive(false); });
    }
}

[System.Serializable]
public class CardSprite
{
    public CardShape shape;
    public Sprite[] sprite;
}

public class BlackJackUI : MinigameUI
{
    [Header("Lobby")]
    public MsgPanel errorMsg_Lobby;

    [Header("Betting")]
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI remainChipText;
    public TextMeshProUGUI betTimesText_Betting;
    public MsgPanel errorMsg_Betting;

    [Header("Play")]
    public MsgPanel winResultMsg;
    public MsgPanel defeatResultMsg;

    [Header("Parameter")]
    public List<Image> playerHand;
    public List<Image> dealerHand;
    public float errorMsgSecond;
    public float resultSecond;
    public float drawSecond;
    public float openSecond;
    public float throwSecond;
    public float throwInterval;
    public float resultInterval;
    public float cardSpace;
    public float openSpace;

    [Header("Transform")]
    public RectTransform playerTransform;
    public RectTransform dealerTransform;
    public RectTransform deckTransform;
    public RectTransform playerChipTransform;
    public RectTransform dealerChipTransform;
    public Vector2 chipMinPos;
    public Vector2 chipMaxPos;

    [Header("Card")]
    public CardSprite[] cardSprites;
    public Sprite backCard;

    private int betChip;
    private List<RectTransform> chips = new List<RectTransform>();
    private List<Card> deck = new List<Card>();
    private List<Card> player = new List<Card>();
    private List<Card> dealer = new List<Card>();
    private List<int> pickIndex = new List<int>();
    private Dictionary<GameResult, int> reward = new Dictionary<GameResult, int>();
    private Sprite[,] sprites;
    private bool canDraw;
    private bool canStart;

    private void Start()
    {
        sprites = new Sprite[4, 13];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                deck.Add(new Card((CardShape)i, j));
                sprites[i, j - 1] = cardSprites[i].sprite[j - 1];
            }
        }

        reward[GameResult.Player_BlackJack] = 3;
        reward[GameResult.Player_Win] = 2;
        reward[GameResult.Dealer_BlackJack] = -2;
        reward[GameResult.Dealer_Win] = -1;
        reward[GameResult.Draw] = 0;

        canStart = true;
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        betChip = building.betChip;
    }

    #region SetFunc
    protected override void SetValue()
    {
        //TODO - 나중에 연출 여기다가 넣으면 될듯
        base.SetValue();
    }

    protected override void SetUI(MinigameState state)
    {
        switch(state)
        {
            case MinigameState.Lobby:
                base.SetValue();
                break;
            case MinigameState.Betting:
                betChipText.text = betChip.ToString();
                remainChipText.text = (ChipManager.instance.curChip - betChip).ToString();
                betTimesText_Betting.text = curGameBuilding.betTimes.ToString();
                break;
            case MinigameState.Play:
                break;
        }
    }

    private void SetResultPanel(GameResult result)
    {
        switch (result)
        {
            case GameResult.Player_BlackJack:
                winResultMsg.OnMessage("!! Player BlackJack !!", resultSecond);
                break;
            case GameResult.Player_Win:
                winResultMsg.OnMessage("!! Player Win !!", resultSecond);
                break;
            case GameResult.Dealer_BlackJack:
                defeatResultMsg.OnMessage("!! Dealerb BlackJack !!", resultSecond);
                break;
            case GameResult.Dealer_Win:
                defeatResultMsg.OnMessage("!! Dealer Win !!", resultSecond);
                break;
            case GameResult.Draw:
                defeatResultMsg.OnMessage("!! Draw !!", resultSecond);
                break;
        }

        StartCoroutine(ReturnToLobby(resultSecond, result));
    }

    private void ResetCard()
    {
        pickIndex.Clear();
        player.Clear();
        dealer.Clear();

        for(int i = 0;i < playerHand.Count; i++)
            PoolSystem.instance.messagePool.TakeToPool<Image>("Card", playerHand[i]);

        for (int i = 0; i < dealerHand.Count; i++)
            PoolSystem.instance.messagePool.TakeToPool<Image>("Card", dealerHand[i]);

        playerHand.Clear();
        dealerHand.Clear();

        canDraw = true;
    }

    private void ResetChip()
    {
        for (int i = 0; i < chips.Count; i++)
            PoolSystem.instance.messagePool.TakeToPool<RectTransform>("Chip", chips[i]);

        chips.Clear();
    }
    #endregion

    #region OnClick
    public void OnClickStartGame()
    {
        if (curGameBuilding.betTimes == 0)
            errorMsg_Lobby.OnMessage("실행 가능 횟수를\n모두 소진하였습니다.", errorMsgSecond);
        else if (ChipManager.instance.curChip < curGameBuilding.betChip)
            errorMsg_Lobby.OnMessage("해당 미니 게임을 하기 위한\n칩이 부족합니다.", errorMsgSecond);
        else if (!canStart) return;
        else
        {
            curGameBuilding.betTimes--;
            SetState(MinigameState.Betting);
            StartCoroutine(ThrowChip(curGameBuilding.betChip, true));
        }
    }

    public void OnClickBetChip(int amount)
    {
        if(betChip + amount > ChipManager.instance.curChip)
            errorMsg_Betting.OnMessage("배팅하기 위한\n칩이 부족합니다.", errorMsgSecond);
        else if (betChip + amount < curGameBuilding.betChip)
            errorMsg_Betting.OnMessage("기본 판돈이하로\n칩을 회수할 수 없습니다.", errorMsgSecond);
        else
        {
            betChip += amount;
            SetUI(MinigameState.Betting);
            StartCoroutine(ThrowChip(amount, true));
        }
    }

    public void OnClickPlayGame()
    {
        if (!ChipManager.instance.PayChip(betChip)) return;
        SetState(MinigameState.Play);
        StartCoroutine(StartBlackJack());
    }

    public void OnClickDrawCard()
    {
        if (canDraw)
            StartCoroutine(PlayerTurn());
    }

    public void OnClickCheckResult()
    {
        if (!canDraw) return;

        canDraw = false;
        StartCoroutine(DealerTurn());
    }
    #endregion

    #region GameLogic
    IEnumerator StartBlackJack()
    {
        yield return StartCoroutine(ThrowChip(-betChip, false));

        canDraw = false;
        player.Add(PickCard());
        player.Add(PickCard());
        dealer.Add(PickCard());
        dealer.Add(PickCard());

        playerHand.Add(InitImage());
        playerHand.Add(InitImage());
        dealerHand.Add(InitImage());
        dealerHand.Add(InitImage());

        AlignCard(true, 0);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(true, 1);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(false, 0);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(false, 1);
        yield return new WaitForSeconds(drawSecond / 4.0f);

        OpenCard(playerHand[0], player[0]);
        yield return new WaitForSeconds(openSecond / 4.0f);
        OpenCard(playerHand[1], player[1]);
        yield return new WaitForSeconds(openSecond / 4.0f);
        OpenCard(dealerHand[0], dealer[0]);
        yield return new WaitForSeconds(openSecond / 4.0f);

        canDraw = true;
    }

    private Card PickCard()
    {
        int ranIdx = Random.Range(0, 52);
        while (pickIndex.Contains(ranIdx))
        {
            ranIdx = Random.Range(0, 52);
        }
        pickIndex.Add(ranIdx);
        return deck[ranIdx];
    }

    private int CalculateResult(List<Card> cards)
    {
        int aceCount = 0;
        int result = 0;

        foreach(Card card in cards)
        {
            if (card.number == 1)
            {
                result += 11;
                aceCount++;
            }
            else
                result += Mathf.Min(card.number, 10);
        }

        for (int i = 0;i < aceCount; i++)
        {
            if (result > 21)
                result -= 10;
            else
                break;
        }

        return result;
    }

    private void GetReward(GameResult result)
    {
        canDraw = false; ;
        ChipManager.instance.curChip = Mathf.Max(ChipManager.instance.curChip + reward[result] * betChip, 0);
    }

    private GameResult GetResult()
    {
        int resP = CalculateResult(player);
        int resD = CalculateResult(dealer);
        GameResult result;

        if (resD == resP)
            result = GameResult.Draw;
        else if (resP > resD)
        {
            if (resP == 21)
                result = GameResult.Player_BlackJack;
            else
                result = GameResult.Player_Win;
        }
        else
        {
            if (resD == 21)
                result = GameResult.Dealer_BlackJack;
            else
                result = GameResult.Dealer_Win;
        }
        return result;
    }
    #endregion

    #region Direction
    IEnumerator ReturnToLobby(float second, GameResult result)
    {
        yield return new WaitForSeconds(second);

        int rewardChip = betChip * reward[result];
        betChip = curGameBuilding.betChip;
        SetState(MinigameState.Lobby);

        ResetCard();

        canStart = false;
        StartCoroutine(ThrowChip(Mathf.Abs(rewardChip), rewardChip > 0 ? false : true));
        yield return new WaitForSeconds(resultInterval);
        StartCoroutine(ThrowChip(Mathf.Abs(rewardChip) * -1, rewardChip > 0 ? true : false));
        canStart = true;
    }

    IEnumerator DrawCard(bool isPlayer, bool isOpen)
    {
        canDraw = false;

        List<Card> cards = isPlayer ? player : dealer;
        List<Image> images = isPlayer ? playerHand : dealerHand;

        Card newCard = PickCard();
        cards.Add(newCard);

        Image newImage = InitImage();
        images.Add(newImage);

        AlignCard(isPlayer);
        yield return new WaitForSeconds(drawSecond);

        if (isOpen)
        {
            OpenCard(newImage, newCard);
            yield return new WaitForSeconds(openSecond);
        }

        canDraw = true;
    }

    IEnumerator PlayerTurn()
    {
        yield return StartCoroutine(DrawCard(true, true));

        if (CalculateResult(player) > 21)
        {
            defeatResultMsg.OnMessage("!! Player Burst !!", resultSecond);
            GetReward(GameResult.Dealer_Win);
            StartCoroutine(ReturnToLobby(resultSecond, GameResult.Dealer_Win));
        }
    }

    IEnumerator DealerTurn()
    {
        OpenCard(dealerHand[1], dealer[1]);
        yield return new WaitForSeconds(openSecond);

        while (CalculateResult(dealer) <= 16)
        {
            yield return StartCoroutine(DrawCard(false, true));
        }

        if (CalculateResult(dealer) > 21)
        {
            winResultMsg.OnMessage("!! Dealer Burst !!", resultSecond);
            GetReward(GameResult.Player_Win);
            StartCoroutine(ReturnToLobby(resultSecond, GameResult.Player_Win));
        }
        else
        {
            GameResult result = GetResult();
            GetReward(result);
            SetResultPanel(result);
        }
    }

    IEnumerator ThrowChip(int amount, bool isPlayer)
    {
        for (int i = 0; i < Mathf.Abs(amount); i++)
        {
            if (amount > 0)
                ThrowChip(isPlayer);
            else
                StartCoroutine(ReturnChip(isPlayer));
            yield return new WaitForSeconds(throwInterval);
        }
    }

    IEnumerator ReturnChip(bool isPlayer)
    {
        Vector3 chipPos = isPlayer ? playerChipTransform.localPosition : dealerChipTransform.localPosition;

        int ranIdx = Random.Range(0, chips.Count);

        RectTransform transform = chips[ranIdx];
        transform.DOLocalMove(chipPos, throwSecond).SetEase(Ease.OutCubic);
        chips.RemoveAt(ranIdx);

        yield return new WaitForSeconds(throwSecond);

        PoolSystem.instance.messagePool.TakeToPool<RectTransform>("Chip", transform);
    }

    private Image InitImage()
    {
        Image newImage = PoolSystem.instance.messagePool.GetFromPool<Image>("Card");
        newImage.sprite = backCard;
        newImage.rectTransform.localPosition = deckTransform.localPosition;
        return newImage;
    }

    private void ThrowChip(bool isPlayer)
    {
        Vector3 chipPos = isPlayer ? playerChipTransform.localPosition : dealerChipTransform.localPosition;
        float xPos = Random.Range(chipMinPos.x, chipMaxPos.x);
        float yPos = Random.Range(chipMinPos.y, chipMaxPos.y);

        RectTransform transform = PoolSystem.instance.messagePool.GetFromPool<RectTransform>("Chip");
        transform.localPosition = chipPos;
        transform.DOLocalMove(new Vector3(xPos, yPos, 0), throwSecond).SetEase(Ease.OutCubic);
        chips.Add(transform);
    }

    private void AlignCard(bool isPlayer, int idx = -1)
    {
        List<Image> images = isPlayer ? playerHand : dealerHand;
        float width = images[0].rectTransform.sizeDelta.x;
        float minPos = -(width / 2.0f) * (images.Count - 1) - (cardSpace / 2.0f) * (images.Count - 1);
        float space = width + cardSpace;
        float yPos = isPlayer ? playerTransform.localPosition.y : dealerTransform.localPosition.y;

        if (idx == -1)
        {
            for (int i = 0; i < images.Count; i++)
            {
                float xPos = minPos + space * i;
                Vector3 newPos = new Vector3(xPos, yPos, images[i].transform.localPosition.z);
                images[i].transform.DOLocalMove(newPos, drawSecond);
            }
        }
        else
        {
            float xPos = minPos + space * idx;
            Vector3 newPos = new Vector3(xPos, yPos, images[idx].transform.localPosition.z);
            images[idx].transform.DOLocalMove(newPos, drawSecond);
        }
    }

    private void OpenCard(Image image, Card card)
    {
        image.transform.DOLocalMoveX(image.transform.localPosition.x - openSpace, openSecond / 2.0f).OnComplete(() =>
        {
            image.sprite = sprites[(int)card.shape, card.number - 1];
            image.transform.DOLocalMoveX(image.transform.localPosition.x + openSpace, openSecond / 2.0f);
        });
    }
    #endregion
}