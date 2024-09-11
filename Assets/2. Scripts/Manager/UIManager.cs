using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : MonoBehaviour, ISubject
{
    static public UIManager instance;

    #region UIComponent

    public Canvas canvas;
    public InputManager inputManager;

    [Header("Info")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI debtDayText;
    public TextMeshProUGUI dayOfWeekText;
    public TextMeshProUGUI debtInfoText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI moneyText;
    public EventInfoUI eventInfo;

    [Header("Building")]
    public BuildingIntroUI[] buildingIntros;
    public InfoUI[] infos;
    public ListUI[] lists;

    [Header("CityLevel")]
    public UIElement cityLevelPanel;
    public GameObject[] cityLevels;

    [Header("CityLevelUp")]
    public CityLevelUpUI cityLevelUp;

    [Header("Roulette")]
    public EventRouletteUI eventRoulette;

    [Header("Message")]
    public GameObject errorMessagePrefab;
    public GameObject happinessMessagePrefab;
    public GameObject eventMessagePrefab;
    public DestroyBuildingUI destroyMessage;

    [Header("Cost")]
    public CostUI costInfo;

    [Header("Event Notify")]
    public EventNotifyUI eventNotify;

    [Header("Func")]
    public UIElement construct;
    public UIElement etcFunc;

    [Header("LockButton")]
    public UILockButton[] lockButtons;

    [Header("MouseOver")]
    public IconNameMouseOver[] mouseOvers;

    [Header("Debt")]
    public CreditScorePanel creditScorePanel;
    public DDayUI[] ddays;

    [Header("Panel")]
    public GameObject topPanel;
    public GameObject bottomPanel;
    public GameObject inputPanel;

    [Header("Menu")]
    public UIElement menu;
    public UIElement setting;

    [Header("Phone")]
    public PhoneUI phone;
    public GamePhoneUI gamePhone;
    public GameFinishUI gameFinish;

    [Header("Tutorial")]
    public TutorialUI tutorial;

    #endregion

    private Building targetBuilding;
    private List<IObserver> observers = new List<IObserver>();
    private string[] weekStr = { "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일" };
    private string[] weekShortStr = { "일", "월", "화", "수", "목", "금", "토" };

    //public int NewsHappiness;
    //private int previousHappiness;
    //private bool UpdateNews = false;
    //public RectTransform imageRectTransform;
    //public TextMeshProUGUI textComponent;
    //public float animationDuration = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        UpdateDailyInfo();
        Setmoney();
        SetCreditScorePanel();
        InitObserver();
        notifyObserver(EventState.LockButton);
        SetEventInfo(EventManager.instance.curEvents.ToArray());

        //NewsHappiness = (int)RoutineManager.instance.cityHappiness;
        //previousHappiness = NewsHappiness;
    }

    #region NewsMessage
    /*
    void Update()
    {
        //lastCityHappiness = RoutineManager.instance.cityHappiness;
        //int[] happiness = BuildingSpawner.instance.GetBuildingsHappiness();
        //lastBuildingHappiness[(int)BuildingType.Residential] = happiness[(int)BuildingType.Residential];
        //lastBuildingHappiness[(int)BuildingType.Commercial] = happiness[(int)BuildingType.Commercial];
        //lastBuildingHappiness[(int)BuildingType.Culture] = happiness[(int)BuildingType.Culture];
        ////lastBuildingHappiness[(int)BuildingType.Service] = happiness[(in ...
        //lastCityMoney = ShopManager.instance.Money;
        //lastTotalTax = CalculateTotalTax();
        //lastTotalSpend = CalculateTotalSpend();
        //lastBuildingTypeCount = BuildingSpawner.instance.buildingTypeCount;
        //lastDay = RoutineManager.instance.day;

        //CheckCityHappiness();
        //CheckBuildingHappiness();
        //CheckCityMoney();
        //CheckTotalTax();
        //CheckTotalSpend();
        //CheckBuildingCounts();
        //CheckDayChange();
    }

    public Text[] messageTexts;
    private Queue<string> messageQueue = new Queue<string>();
    private float messageDuration = 5.0f;

    private float lastCityHappiness;
    private int[] lastBuildingHappiness = new int[4];
    private int lastCityMoney;
    private int lastTotalTax;
    private int lastTotalSpend;
    private int[] lastBuildingTypeCount = new int[4];
    private DateTime lastDay;

    void CheckCityHappiness()
    {
        float currentHappiness = RoutineManager.instance.cityHappiness;
        if (currentHappiness != lastCityHappiness)
        {
            AddMessage($"City happiness changed: {currentHappiness}");
            lastCityHappiness = currentHappiness;
        }
    }

    void CheckBuildingHappiness()
    {
        int[] happiness = BuildingSpawner.instance.GetBuildingsHappiness();
        for (int i = 0; i < lastBuildingHappiness.Length; i++)
        {
            if (happiness[i] != lastBuildingHappiness[i])
            {
                BuildingType type = (BuildingType)i;
                AddMessage($"{type} building happiness changed: {happiness[i]}");
                lastBuildingHappiness[i] = happiness[i];
            }
        }
    }

    void CheckCityMoney()
    {
        int currentMoney = ShopManager.instance.Money;
        if (currentMoney != lastCityMoney)
        {
            AddMessage($"City money changed: {currentMoney}");
            lastCityMoney = currentMoney;
        }
    }

    void CheckTotalTax()
    {
        int currentTotalTax = CalculateTotalTax();
        if (currentTotalTax != lastTotalTax)
        {
            AddMessage($"Total daily tax income changed: {currentTotalTax}");
            lastTotalTax = currentTotalTax;
        }
    }

    void CheckTotalSpend()
    {
        int currentTotalSpend = CalculateTotalSpend();
        if (currentTotalSpend != lastTotalSpend)
        {
            AddMessage($"Total daily spend changed: {currentTotalSpend}");
            lastTotalSpend = currentTotalSpend;
        }
    }

    void CheckBuildingCounts()
    {
        int[] buildingCounts = BuildingSpawner.instance.buildingTypeCount;
        for (int i = 0; i < lastBuildingTypeCount.Length; i++)
        {
            if (buildingCounts[i] != lastBuildingTypeCount[i])
            {
                BuildingType type = (BuildingType)i;
                AddMessage($"{type} buildings count changed: {buildingCounts[i]}");
                lastBuildingTypeCount[i] = buildingCounts[i];
            }
        }
    }

    void CheckDayChange()
    {
        DateTime currentDay = RoutineManager.instance.day;
        if (currentDay != lastDay)
        {
            AddMessage($"Day changed: {currentDay.ToShortDateString()}");
            lastDay = currentDay;
        }
    }

    void AddMessage(string message)
    {
        if (messageQueue.Count >= messageTexts.Length)
        {
            messageQueue.Dequeue();
        }
        messageQueue.Enqueue(message);
        UpdateMessages();
        StartCoroutine(RemoveOldestMessage());
    }

    private void UpdateMessages()
    {
        int i = 0;
        foreach (string msg in messageQueue)
        {
            if (i < messageTexts.Length)
            {
                messageTexts[i].text = msg;
                i++;
            }
        }
        for (; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = "";
        }
    }

    IEnumerator RemoveOldestMessage()
    {
        yield return new WaitForSeconds(messageDuration);
        if (messageQueue.Count > 0)
        {
            messageQueue.Dequeue();
            UpdateMessages();
        }
    }

    int CalculateTotalTax()
    {
        return ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome;
    }

    int CalculateTotalSpend()
    {
        return ServiceBuilding.income + ResidentialBuilding.bonusCost + ServiceBuilding.bonusCost + Tile.income;
    }
    */
    #endregion

    #region SetValue

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
    }

    public void UpdateDailyInfo()
    {
        DateTime curDay = RoutineManager.instance.day;
        dayText.text = curDay.ToString("yyyy/MM/dd");
        dayOfWeekText.text = weekStr[(int)curDay.DayOfWeek];
        SetDebt();
        SetDebtInfo();
        SetDDay(curDay);
    }

    public void Setmoney()
    {
        moneyText.text = GetCommaText(ShopManager.instance.Money) + "원";
    }

    public void DailyMoneyUpdate(int prevMoney, int nextMoney, float second)
    {
        StartCoroutine(moneyText.gameObject.GetComponent<MoneyTextUI>().CalculateMoney(prevMoney, nextMoney, second));
    }

    public void SetDebtInfo()
    {
        if (CityLevelManager.instance.levelIdx == CityLevelManager.instance.level.Length - 1) return;

        DateTime curDay = RoutineManager.instance.day;
        DateTime payDay = RoutineManager.instance.GetPayDay();
        int dDay = payDay.DayOfYear - curDay.DayOfYear - 1;
        if (RoutineManager.instance.debt != 0)
            debtDayText.text = "<size=300>D-" + dDay.ToString() + "</size>";
        else
            debtDayText.text = "<size=200>납부 완료</size>";

        int maxWeek = CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].debtWeek;
        int curWeek = RoutineManager.instance.GetWeekOfYear() - CityLevelManager.instance.GetPrefixSumWeek();

        debtInfoText.text = curWeek.ToString() + " / " + maxWeek.ToString();
    }

    public void SetDebt()
    {
        debtText.text = GetCommaText(RoutineManager.instance.debt) + "원";
    }

    public void SetDDay(DateTime curDay)
    {
        int dayOfWeek = curDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)curDay.DayOfWeek;
        DateTime day = RoutineManager.instance.day;
        DateTime payDay = RoutineManager.instance.GetPayDay();
        int dDay = payDay.DayOfYear - day.DayOfYear - 1;

        if (dayOfWeek >= 5 && !RoutineManager.instance.isPay && dDay < 7)
        {
            ddays[dayOfWeek - 5].gameObject.SetActive(true);
        }
    }

    public void SetCreditScorePanel()
    {
        creditScorePanel.SetValue();
    }

    public void SetBuildingIntroValue()
    {
        int idx = GetBuildingIndex();
        buildingIntros[idx].SetValue(targetBuilding);
    }

    public void SetEventNotifyValue(Building building)
    {
        eventNotify.SetValue(building);
    }

    public void SetBuildingListValue(int index)
    {
        lists[index].SetValue(index);
    }

    public void SetEventInfo(Event[] curEvents)
    {
        eventInfo.SetValue(curEvents);
    }

    #endregion

    #region PopUp

    public void SetRoulettePopUp(int[] ranEvents)
    {
        eventRoulette.SetRandomEvent(ranEvents);
    }

    public void SetInfoPopUp(int typeIndex, int index)
    {
        infos[typeIndex].SetValue(index);
    }

    public void SetBuildingIntroPopUp(Building building = null)
    {
        if (eventNotify.gameObject.activeSelf)
            SetEventNotifyValue(building);

        if (building == null)
        {
            foreach (BuildingIntroUI introUI in buildingIntros)
                introUI.gameObject.SetActive(false);
        }
        else
        {
            targetBuilding = building;
            int idx = GetBuildingIndex();

            for (int i = 0; i < buildingIntros.Length; i++)
            {
                if (i == idx)
                {
                    buildingIntros[i].gameObject.SetActive(true);
                    buildingIntros[i].OnUI(building);

                }
                else
                    buildingIntros[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetEventNotifyPopUp(bool active)
    {
        eventNotify.gameObject.SetActive(active);

        if (active)
            eventNotify.Init();
    }

    public void SetErrorPopUp(string[] str, Vector3 position)
    {
        TemporayUI message = Instantiate(errorMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(str, position);
        AkSoundEngine.PostEvent("Play_Deny_01", gameObject);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        if (eventRoulette.gameObject.activeSelf) return;

        string[] str = { amount < 0 ? "<sprite=1> <sprite=5>" : "<sprite=3> <sprite=6>" };

        TemporayUI message = Instantiate(happinessMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(str, position);
    }

    public void SetEventTempPopUp(Event curEvent, Vector3 position)
    {
        TemporayUI message = Instantiate(eventMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(curEvent, position);

        if (curEvent.rewardType == RewardType.Penalty)
            AkSoundEngine.PostEvent("Play_POPUP_Negative", gameObject);
        else if (curEvent.rewardType == RewardType.Reward)
            AkSoundEngine.PostEvent("Play_POPUP_Positive", gameObject);
    }

    public void SetCostPopUp(Transform transform = null, int cost = 0)
    {
        if (transform == null)
        {
            costInfo.gameObject.SetActive(false);
        }
        else
            costInfo.OnUI(cost, transform.position);
    }

    public void SetAllPopUp()
    {
        if (ShopManager.instance.buyState == BuyState.BuyTile)
            ShopManager.instance.SetTargetObject(null, Color.green, Color.red);
        else if (ShopManager.instance.buyState == BuyState.SellBuilding)
            ShopManager.instance.SetTargetObject(null, Color.red, Color.white);

        ShopManager.instance.ChangeState(BuyState.None);
    }

    public void SetCityLevel()
    {
        int idx = CityLevelManager.instance.levelIdx;
        for (int i = 0;i < CityLevelManager.instance.level.Length; i++)
        {
            if (i <= idx)
                cityLevels[i].SetActive(true);
            else
                cityLevels[i].SetActive(false);
        }
    }

    #endregion

    #region OnClick

    public void OnClickConstructButton()
    {
        notifyObserver(EventState.Construct);
    }

    public void OnClickEtcFuncButton()
    {
        notifyObserver(EventState.EtcFunc);
    }

    public void OnClickBuyPopUp(int index)
    {
        for (int i = 0; i < lists.Length; i++)
        {
            if (i == index)
            {
                lists[i].gameObject.SetActive(!lists[i].gameObject.activeSelf);
                lists[i].SetValue(i);
            }
            else
                lists[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < infos.Length; i++)
            infos[i].gameObject.SetActive(false);
    }

    public void OnClickTileBuild(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuildTile, index);
    }

    public void OnClickBuildingBuy(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuyBuilding, index);
    }

    public void OnClickBuildingSell()
    {
        ShopManager.instance.ChangeState(BuyState.SellBuilding);
    }

    public void OnClickTileBuy()
    {
        ShopManager.instance.ChangeState(BuyState.BuyTile);
    }

    public void OnClickOptionBuy(int index)
    {
        if (ShopManager.instance.BuyOption((OptionType)index))
        {
            if (ShopManager.instance.buyState == BuyState.SolveBuilding)
            {
                int idx = GetBuildingIndex();
                buildingIntros[idx].SetValue(targetBuilding);
            }
            else if (eventNotify.curIndex < EventManager.instance.eventBuildings.Count)
                eventNotify.SetValue(EventManager.instance.eventBuildings[eventNotify.curIndex]);
        }
    }

    public void OnClickNextDay()
    {
        if (eventRoulette.gameObject.activeSelf) return;

        notifyObserver(EventState.None);
        RoutineManager.instance.DailyUpdate();
        AkSoundEngine.PostEvent("Play_Turn_Move_Clock_001", gameObject);
    }

    public void OnClickSolveEvent(int index)
    {
        ShopManager.instance.SolveEvent(index);
    }

    public void OnClickTileColorMode()
    {
        notifyObserver(EventState.TileColor);
    }

    public void OnClickEventHighLight()
    {
        if (BuildingSpawner.instance.GetEventBuildingCount() <= 0)
        {
            string[] str = { "문제가 발생한 건물이 없습니다." };
            SetErrorPopUp(str, transform.position);
            return;
        }
        notifyObserver(EventState.EventIcon);
    }

    public void OnClickEventNotify()
    {
        if (BuildingSpawner.instance.GetEventBuildingCount() <= 0) 
        {
            string[] str = { "문제가 발생한 건물이 없습니다." };
            SetErrorPopUp(str, transform.position);
            return;
        }

        notifyObserver(EventState.EventNotify);
    }

    public void OnClickEventNotifyNext(bool isRight)
    {
        if (EventManager.instance.eventBuildings.Count == 0)
            return;

        eventNotify.NextBuilding(isRight);
    }

    public void OnClickEventRoulette()
    {
        if (eventRoulette.state == RouletteState.While || eventRoulette.state == RouletteState.Before) return;
        if (!EventManager.instance.PayRoulleteCost()) return;
        eventRoulette.OnButtonClick();
    }

    public void OnClickSlotMachineButton()
    {
        if (EventManager.instance.CheckEventCondition())
        {
            notifyObserver(EventState.SlotMachine);
        }
        else
        {
            string[] str = { "도시 규모가 작아 슬롯머신을 작동할 수 없습니다." };
            SetErrorPopUp(str, transform.position);
        }
    }

    public void OnClickDeptDocButton()
    {
        if (RoutineManager.instance.debt != 0)
            notifyObserver(EventState.DebtDoc);
    }

    public void OnClickCityLevelButton()
    {
        notifyObserver(EventState.CityLevel);
        SetCityLevel();
    }

    public void OnClickCloseButton()
    {
        notifyObserver(EventState.None);
    }

    public void OnClickSettingButton()
    {
        notifyObserver(EventState.Setting);
    }

    public void OnClickPhoneButton()
    {
        notifyObserver(EventState.Phone);
    }

    #endregion


    public void MovePanelAnim(float second, bool isUp)
    {
        float sign = isUp ? 1 : -1;
        topPanel.transform.DOLocalMoveY(topPanel.transform.localPosition.y + 210.0f * sign, second);
        bottomPanel.transform.DOLocalMoveY(bottomPanel.transform.localPosition.y + -210.0f * sign, second);
    }

    private int GetBuildingIndex()
    {
        int idx = targetBuilding.type == BuildingType.Residential ? 0 : 1;
        return idx;
    }

    #region Observer
    public void addObserver(IObserver observer)
    {
        if (observer != null)
            observers.Add(observer);
        else
            Debug.LogError("Null Observer : " + observer.ToString());
    }

    public void removeObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void notifyObserver(EventState state)
    {
        foreach(IObserver observer in observers)
        {
            observer.Notify(state);
        }
    }

    private void InitObserver()
    {
        addObserver(inputManager);
        addObserver(ShopManager.instance);
        addObserver(Grid.instance);
        addObserver(BuildingSpawner.instance);

        addObserver(cityLevelPanel);
        addObserver(eventNotify);
        addObserver(eventRoulette);

        addObserver(costInfo);
        addObserver(construct);
        addObserver(etcFunc);

        addObserver(menu);
        addObserver(setting);

        addObserver(destroyMessage);
        addObserver(phone);
        addObserver(gamePhone);
        addObserver(gameFinish);

        addObserver(tutorial);

        foreach (UILockButton lockButton in lockButtons)
        {
            addObserver(lockButton);
        }

        foreach (IconNameMouseOver mouseOver in mouseOvers)
        {
            addObserver(mouseOver);
        }
    }
    #endregion
}