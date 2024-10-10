using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

public class BlackJackUI : MinigameUI
{
    [Header("Panels")]
    public GameObject[] panels;

    [Header("Lobby")]
    public TextMeshProUGUI baseChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI betTimesText_Lobby;

    [Header("Betting")]
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI remainChipText;
    public TextMeshProUGUI betTimesText_Betting;

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
        reward[GameResult.Draw] = -1;
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        SetState(BlackJackState.Lobby);
        betChip = building.betChip;
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
    }
    #endregion

    #region OnClick
    public void OnClickStartGame()
    {
        SetState(BlackJackState.Betting);
    }

    public void OnClickQuitGame()
    {
        UIManager.instance.notifyObserver(EventState.None);
    }

    public void OnClickBetChip(int amount)
    {
        if(betChip + amount > ChipManager.instance.curChip)
        {
            //TODO - Error Msg
            return;
        }
        else if (betChip + amount < curGameBuilding.betChip)
        {
            //TODO - Error Msg
            return;
        }

        betChip += amount;
        SetUI(BlackJackState.Betting);

        //TODO - 칩 던지는 연출있으면 좋을거 같음
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

        if (CalculateResult(player) > 21)
        {
            //TODO - Player Burst!!
            GetReward(GameResult.Dealer_Win);
        }
    }

    public void OnClickCheckResult()
    {
        DrawDealer();
        GetReward(GetResult());
    }
    #endregion

    #region GameLogic
    private void StartBlackJack()
    {
        pickIndex.Clear();
        player.Clear();
        player.Clear();

        player.Add(DrawCard());
        player.Add(DrawCard());

        dealer.Add(DrawCard());
        dealer.Add(DrawCard());
    }

    private void DrawDealer()
    {
        while(CalculateResult(dealer) <= 16)
        {
            dealer.Add(DrawCard());
        }

        if (CalculateResult(dealer) > 21)
        {
            //TODO - Dealer Burst!!
            GetReward(GameResult.Player_Win);
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
        ChipManager.instance.curChip = Mathf.Max(ChipManager.instance.curChip + reward[result], 0);
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
    #endregion
}