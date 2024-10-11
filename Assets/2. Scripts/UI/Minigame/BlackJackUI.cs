using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BlackJackState { Lobby, Betting, Play }
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

public class BlackJackUI : MinigameUI
{
    [Header("Panels")]
    public GameObject[] panels;

    [Header("Lobby")]
    public TextMeshProUGUI baseChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI betTimesText_Lobby;
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
    public float errorMsgSecond;
    public float resultSecond;

    private int betChip;
    private List<Card> deck = new List<Card>();
    private List<Card> player = new List<Card>();
    private List<Card> dealer = new List<Card>();
    private List<int> pickIndex = new List<int>();
    private Dictionary<GameResult, int> reward = new Dictionary<GameResult, int>();

    private void Start()
    {
        for (int i = 0;i < 4; i++)
            for (int j = 1; j <= 13; j++)
                deck.Add(new Card((CardShape)i, j));

        reward[GameResult.Player_BlackJack] = 3;
        reward[GameResult.Player_Win] = 2;
        reward[GameResult.Dealer_BlackJack] = -2;
        reward[GameResult.Dealer_Win] = -1;
        reward[GameResult.Draw] = 0;
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        SetState(BlackJackState.Lobby);
        betChip = building.betChip;
        InputManager.SetCanInput(false);
    }

    #region SetFunc
    public override void SetValue()
    {
        //TODO - 나중에 연출 여기다가 넣으면 될듯
    }

    private void SetUI(BlackJackState state)
    {
        switch(state)
        {
            case BlackJackState.Lobby:
                baseChipText.text = curGameBuilding.betChip.ToString();
                curChipText.text = ChipManager.instance.curChip.ToString();
                betTimesText_Lobby.text = curGameBuilding.betTimes.ToString();
                break;
            case BlackJackState.Betting:
                betChipText.text = betChip.ToString();
                remainChipText.text = (ChipManager.instance.curChip - betChip).ToString();
                betTimesText_Betting.text = curGameBuilding.betTimes.ToString();
                break;
            case BlackJackState.Play:
                break;
        }
    }

    private void SetState(BlackJackState state)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == (int)state);
        }

        SetUI(state);
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

        StartCoroutine(ReturnToLobby(resultSecond));
    }
    #endregion

    #region OnClick
    public void OnClickStartGame()
    {
        if (curGameBuilding.betTimes == 0)
            errorMsg_Lobby.OnMessage("실행 가능 횟수를\n모두 소진하였습니다.", errorMsgSecond);
        else if(ChipManager.instance.curChip < curGameBuilding.betChip)
            errorMsg_Lobby.OnMessage("해당 미니 게임을 하기 위한\n칩이 부족합니다.", errorMsgSecond);
        else
        {
            curGameBuilding.betTimes--;
            SetState(BlackJackState.Betting);
        }
    }

    public void OnClickQuitGame()
    {
        InputManager.SetCanInput(true);
        UIManager.instance.notifyObserver(EventState.None);
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
            SetUI(BlackJackState.Betting);

            //TODO - 칩 던지는 연출있으면 좋을거 같음
        }
    }

    public void OnClickPlayGame()
    {
        if (!ChipManager.instance.PayChip(betChip)) return;
        SetState(BlackJackState.Play);
        StartBlackJack();
    }

    public void OnClickDrawCard()
    {
        player.Add(DrawCard());
        PrintHand(player);

        if (CalculateResult(player) > 21)
        {
            defeatResultMsg.OnMessage("!! Player Burst !!", resultSecond);
            GetReward(GameResult.Dealer_Win);
            StartCoroutine(ReturnToLobby(resultSecond));
        }
    }

    public void OnClickCheckResult()
    {
        DrawDealer();
    }
    #endregion

    #region GameLogic
    private void StartBlackJack()
    {
        pickIndex.Clear();
        player.Clear();
        dealer.Clear();

        player.Add(DrawCard());
        player.Add(DrawCard());

        dealer.Add(DrawCard());
        dealer.Add(DrawCard());

        PrintHand(player);
        PrintHand(dealer);
    }

    private void DrawDealer()
    {
        while(CalculateResult(dealer) <= 16)
        {
            dealer.Add(DrawCard());
        }
        PrintHand(dealer);

        if (CalculateResult(dealer) > 21)
        {
            winResultMsg.OnMessage("!! Dealer Burst !!", resultSecond);
            GetReward(GameResult.Player_Win);
            StartCoroutine(ReturnToLobby(resultSecond));
        }
        else
        {
            GameResult result = GetResult();
            GetReward(result);
            SetResultPanel(result);
        }
    }

    private Card DrawCard()
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
        ChipManager.instance.curChip = Mathf.Max(ChipManager.instance.curChip + reward[result] * betChip, 0);
    }

    private GameResult GetResult()
    {
        int resP = CalculateResult(player);
        int resD = CalculateResult(dealer);

        if (resD == resP)
            return GameResult.Draw;
        else if (resP > resD)
        {
            if (resP == 21)
                return GameResult.Player_BlackJack;
            else
                return GameResult.Player_Win;
        }
        else
        {
            if (resD == 21)
                return GameResult.Dealer_BlackJack;
            else
                return GameResult.Dealer_Win;
        }
    }

    private void PrintHand(List<Card> cards)
    {
        string res = "";

        foreach(Card card in cards)
        {
            res += card.shape.ToString() + " " + card.number.ToString() + " / ";
        }

        res += "result : " + CalculateResult(cards);

        Debug.Log(res);
    }
    #endregion

    #region Direction
    IEnumerator ReturnToLobby(float second)
    {
        yield return new WaitForSeconds(second);

        betChip = curGameBuilding.betChip;
        SetState(BlackJackState.Lobby);
    }
    #endregion
}