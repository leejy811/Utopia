using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
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
}

[System.Serializable]
public class CardSprite
{
    public CardShape shape;
    public Sprite[] sprite;
}

public class BlackJackUI : MinigameUI
{
    [Header("ErrorMessage")]
    public MsgPanel errorMsg;

    [Header("Betting")]
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI remainChipText;
    public TextMeshProUGUI betTimesText_Betting;

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
    public List<RectTransform> chips = new List<RectTransform>();
    private List<Card> deck = new List<Card>();
    private List<Card> player = new List<Card>();
    private List<Card> dealer = new List<Card>();
    private List<int> pickIndex = new List<int>();
    private Dictionary<GameResult, int> reward = new Dictionary<GameResult, int>();
    private Sprite[,] sprites;
    private bool canClick;

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

        canClick = true;
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        betChip = (int)building.values[ValueType.betChip].cur;
    }

    #region SetFunc
    protected override void SetValue()
    {
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
                StartCoroutine(OnMessage(winResultMsg, "!! Player BlackJack !!", resultSecond));
                break;
            case GameResult.Player_Win:
                StartCoroutine(OnMessage(winResultMsg, "!! Player Win !!", resultSecond));
                break;
            case GameResult.Dealer_BlackJack:
                StartCoroutine(OnMessage(defeatResultMsg, "!! Dealer BlackJack !!", resultSecond));
                break;
            case GameResult.Dealer_Win:
                StartCoroutine(OnMessage(defeatResultMsg, "!! Dealer Win !!", resultSecond));
                break;
            case GameResult.Draw:
                StartCoroutine(OnMessage(defeatResultMsg, "!! Draw !!", resultSecond));
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

        canClick = true;
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
            StartCoroutine(OnMessage(errorMsg, "실행 가능 횟수를\n모두 소진하였습니다.", errorMsgSecond));
        else if (ChipManager.instance.curChip < curGameBuilding.values[ValueType.betChip].cur)
            StartCoroutine(OnMessage(errorMsg, "해당 미니 게임을 하기 위한\n칩이 부족합니다.", errorMsgSecond));
        else if (!canClick) return;
        else
        {
            curGameBuilding.betTimes--;
            SetState(MinigameState.Betting);
            AkSoundEngine.PostEvent("Play_BLACKJACK_intro", gameObject);
            StartCoroutine(ThrowChip((int)curGameBuilding.values[ValueType.betChip].cur, throwInterval, true));
        }
    }

    public void OnClickBetChip(int amount)
    {
        if(betChip + amount > ChipManager.instance.curChip)
            StartCoroutine(OnMessage(errorMsg, "배팅하기 위한\n칩이 부족합니다.", errorMsgSecond));
        else if (betChip + amount < curGameBuilding.values[ValueType.betChip].cur)
            StartCoroutine(OnMessage(errorMsg, "기본 판돈이하로\n칩을 회수할 수 없습니다.", errorMsgSecond));
        else if (!canClick) return;
        else
        {
            if (amount > 0) AkSoundEngine.PostEvent("Play_BLACKJACK_bet", gameObject);
            else AkSoundEngine.PostEvent("Play_BLACKJACK_recall", gameObject);
            betChip += amount;
            SetUI(MinigameState.Betting);
            StartCoroutine(ThrowChip(amount, throwInterval, true));
        }
    }

    public void OnClickAllIn()
    {
        if (betChip == ChipManager.instance.curChip)
            StartCoroutine(OnMessage(errorMsg, "배팅하기 위한\n칩이 부족합니다.", errorMsgSecond));
        else if (!canClick) return;
        else
        {
            AkSoundEngine.PostEvent("Play_BLACKJACK_intro", gameObject);
            StartCoroutine(OnMessage(errorMsg, "ALL-IN", errorMsgSecond));
            StartCoroutine(ThrowChip(ChipManager.instance.curChip - betChip, 0.0f, true));
            betChip = ChipManager.instance.curChip;
            SetUI(MinigameState.Betting);
        }
    }

    public void OnClickResetBetting()
    {
        if (!canClick) return;
        AkSoundEngine.PostEvent("Play_BLACKJACK_intro", gameObject);
        StartCoroutine(ThrowChip((betChip - (int)curGameBuilding.values[ValueType.betChip].cur) * -1, throwInterval, true));
        betChip = (int)curGameBuilding.values[ValueType.betChip].cur;
        SetUI(MinigameState.Betting);
    }

    public void OnClickPlayGame()
    {
        if (!canClick) return;
        if (!ChipManager.instance.PayChip(betChip)) return;
        SetState(MinigameState.Play);
        StartCoroutine(StartBlackJack());
    }

    public void OnClickDrawCard()
    {
        if (canClick)
            StartCoroutine(PlayerTurn());
    }

    public void OnClickCheckResult()
    {
        if (!canClick) return;

        canClick = false;
        StartCoroutine(DealerTurn());
    }
    #endregion

    #region GameLogic
    IEnumerator StartBlackJack()
    {
        //칩 Intro
        yield return StartCoroutine(ThrowChip(-betChip, 0.0f, false));

        canClick = false;
        player.Add(PickCard());
        player.Add(PickCard());
        dealer.Add(PickCard());
        dealer.Add(PickCard());

        playerHand.Add(InitImage());
        playerHand.Add(InitImage());
        dealerHand.Add(InitImage());
        dealerHand.Add(InitImage());

        AlignCard(true, 0);
        AkSoundEngine.PostEvent("Play_BLACKJACK_card_put_01", gameObject);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(true, 1);
        AkSoundEngine.PostEvent("Play_BLACKJACK_card_put_01", gameObject);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(false, 0);
        AkSoundEngine.PostEvent("Play_BLACKJACK_card_put_01", gameObject);
        yield return new WaitForSeconds(drawSecond / 4.0f);
        AlignCard(false, 1);
        AkSoundEngine.PostEvent("Play_BLACKJACK_card_put_01", gameObject);
        yield return new WaitForSeconds(drawSecond / 4.0f);

        OpenCard(playerHand[0], player[0]);
        yield return new WaitForSeconds(openSecond / 4.0f);
        OpenCard(playerHand[1], player[1]);
        yield return new WaitForSeconds(openSecond / 4.0f);
        OpenCard(dealerHand[0], dealer[0]);
        yield return new WaitForSeconds(openSecond / 4.0f);

        canClick = true;
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
        canClick = false; ;
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
        betChip = (int)curGameBuilding.values[ValueType.betChip].cur;
        SetState(MinigameState.Lobby);

        ResetCard();
        ResetChip();

        canClick = false;
        AkSoundEngine.PostEvent("Play_BLACKJACK_intro", gameObject);
        StartCoroutine(ThrowChip(Mathf.Abs(rewardChip), throwInterval, rewardChip > 0 ? false : true));
        yield return new WaitForSeconds(throwInterval * rewardChip + throwSecond * 5);
        StartCoroutine(ThrowChip(Mathf.Abs(rewardChip) * -1, 0.0f, rewardChip > 0 ? true : false));
        yield return new WaitForSeconds(throwSecond);
        canClick = true;
    }

    IEnumerator DrawCard(bool isPlayer, bool isOpen)
    {
        canClick = false;

        List<Card> cards = isPlayer ? player : dealer;
        List<Image> images = isPlayer ? playerHand : dealerHand;

        Card newCard = PickCard();
        cards.Add(newCard);

        Image newImage = InitImage();
        images.Add(newImage);

        AlignCard(isPlayer);
        AkSoundEngine.PostEvent("Play_BLACKJACK_card_put_01", gameObject);
        yield return new WaitForSeconds(drawSecond);

        if (isOpen)
        {
            OpenCard(newImage, newCard);
            yield return new WaitForSeconds(openSecond);
        }

        canClick = true;
    }

    IEnumerator PlayerTurn()
    {
        yield return StartCoroutine(DrawCard(true, true));

        if (CalculateResult(player) > 21)
        {
            AkSoundEngine.PostEvent("Play_BLACKJACK_rule_bust", gameObject);
            StartCoroutine(OnMessage(defeatResultMsg, "!! Player Burst !!", resultSecond));
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
            if (CalculateResult(player) == 21)
            {
                StartCoroutine(OnMessage(winResultMsg, "!! Player BlackJack !!", resultSecond));
                GetReward(GameResult.Player_BlackJack);
                ResultSound(GameResult.Player_BlackJack);
                StartCoroutine(ReturnToLobby(resultSecond, GameResult.Player_BlackJack));
            }
            else
            {
                StartCoroutine(OnMessage(winResultMsg, "!! Dealer Burst !!", resultSecond));
                GetReward(GameResult.Player_Win);
                ResultSound(GameResult.Player_Win);
                StartCoroutine(ReturnToLobby(resultSecond, GameResult.Player_Win));
            }
        }
        else
        {
            GameResult result = GetResult();
            GetReward(result);
            ResultSound(result);
            SetResultPanel(result);
        }
    }

    IEnumerator ThrowChip(int amount, float second, bool isPlayer)
    {
        canClick = false;
        if (isPlayer)
            StartCoroutine(PlayAddChip(plusSecond, -amount));
        for (int i = 0; i < Mathf.Abs(amount); i++)
        {
            if (amount > 0)
                ThrowChip(isPlayer);
            else
                StartCoroutine(ReturnChip(isPlayer));
            yield return new WaitForSeconds(second);
        }
        yield return new WaitForSeconds(throwSecond);
        canClick = true;
    }

    IEnumerator ReturnChip(bool isPlayer)
    {
        Debug.Log(chips.Count);
        Vector3 chipPos = isPlayer ? playerChipTransform.localPosition : dealerChipTransform.localPosition;
        
        int ranIdx = Random.Range(0, chips.Count);

        RectTransform transform = chips[ranIdx];
        transform.DOLocalMove(chipPos, throwSecond).SetEase(Ease.OutCubic);
        chips.RemoveAt(ranIdx);

        yield return new WaitForSeconds(throwSecond);

        PoolSystem.instance.messagePool.TakeToPool<RectTransform>("Chip", transform);
    }

    IEnumerator OnMessage(MsgPanel msgPanel, string msg, float second)
    {
        msgPanel.panel.SetActive(true);
        msgPanel.msgText.text = msg;
        yield return new WaitForSeconds(second);
        msgPanel.panel.SetActive(false);
    }

    private void ResultSound(GameResult result)
    {
        switch (result)
        {
            case GameResult.Dealer_Win:
            case GameResult.Dealer_BlackJack:
                //Defeat Sound
                break;
            case GameResult.Player_Win:
                AkSoundEngine.PostEvent("Play_BLACKJACK_rule_win", gameObject);
                break;
            case GameResult.Player_BlackJack:
                AkSoundEngine.PostEvent("Play_BLACKJACK_rule_blackjack", gameObject);
                break;
            case GameResult.Draw:
                AkSoundEngine.PostEvent("Play_BLACKJACK_rule_draw", gameObject);
                break;
        }
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
            AkSoundEngine.PostEvent("Play_BLACKJACK_card_flip", gameObject);
            image.sprite = sprites[(int)card.shape, card.number - 1];
            image.transform.DOLocalMoveX(image.transform.localPosition.x + openSpace, openSecond / 2.0f);
        });
    }
    #endregion
}