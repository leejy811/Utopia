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
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI cityResidentText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI NewsMessage;
    public TextMeshProUGUI NewsMessage2;

    [Header("Building")]
    public BuildingIntroUI[] buildingIntros;
    public InfoUI[] infos;
    public ListUI[] lists;

    [Header("CityLevelUp")]
    public UIElement cityLevelUpPanel;
    public GameObject[] cityLevelUps;

    [Header("Roulette")]
    public EventRouletteUI eventRoulette;

    [Header("Message")]
    public GameObject errorMessagePrefab;
    public GameObject happinessMessagePrefab;

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
    public DebtUI debtDoc;
    public DebtUI receipt;
    public DebtUI reminder;
    public TextMeshProUGUI creditRatingText;
    public Slider debtSlider;
    public UIElement lateReceipt;

    #endregion

    private Building targetBuilding;
    private List<IObserver> observers = new List<IObserver>();
    private string[] weekStr = { "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일" };

    public int NewsHappiness;
    private int previousHappiness;
    private bool UpdateNews = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public RectTransform imageRectTransform;
    public TextMeshProUGUI textComponent;
    public float animationDuration = 0.5f;

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        UpdateDailyInfo();
        NewsHappiness = (int)RoutineManager.instance.cityHappiness;
        previousHappiness = NewsHappiness;

        InitObserver();
        //notifyObserver(EventState.LockButton);
    }

    #region NewsMessage

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
        dayText.text = curDay.ToString("yy") + "/" + curDay.ToString("MM") + "/" + curDay.ToString("dd") + " " + weekStr[(int)curDay.DayOfWeek];
        SetHappiness();
        Setmoney();
        SetCityResident();
        SetDebt();
        SetCreditRating();
    }

    public void SetHappiness()
    {
        happinessText.text = ((int)RoutineManager.instance.cityHappiness).ToString() + "%";
    }

    public void Setmoney()
    {
        moneyText.text = GetCommaText(ShopManager.instance.Money);
    }

    public void SetCityResident()
    {
        cityResidentText.text = GetCommaText(ResidentialBuilding.cityResident);
    }

    public void SetDebt()
    {
        debtText.text = GetCommaText(RoutineManager.instance.debt);
    }

    public void SetDebtSlider(int dayOfWeek)
    {
        debtSlider.value = dayOfWeek / 7.0f - 0.05f;
        ColorBlock colorBlock = debtSlider.colors;
        colorBlock.disabledColor = dayOfWeek < 4 ? Color.green : new Color(1, 0.5f, 0);
        debtSlider.colors = colorBlock;
    }

    public void SetCreditRating()
    {
        creditRatingText.text = RoutineManager.instance.creditRating.ToString();
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
                    buildingIntros[i].SetValue(building);

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

    public void SetErrorPopUp(string massage, Vector3 position)
    {
        TemporayUI message = Instantiate(errorMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        string massage = amount < 0 ? "<sprite=1> <sprite=5>" : "<sprite=3> <sprite=6>";

        TemporayUI message = Instantiate(happinessMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
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

    public void SetCityLevelUpPopUp(int index = 0)
    {
        notifyObserver(EventState.CityLevelUp);
        for (int i = 0; i < cityLevelUps.Length; i++)
        {
            if (i == index)
                cityLevelUps[i].SetActive(true);
            else
                cityLevelUps[i].SetActive(false);
        }
    }

    public void SetAllPopUp()
    {
        if (ShopManager.instance.buyState == BuyState.BuyTile)
            ShopManager.instance.SetTargetObject(null, Color.green, Color.red);
        else if (ShopManager.instance.buyState == BuyState.SellBuilding)
            ShopManager.instance.SetTargetObject(null, Color.red, Color.white);

        ShopManager.instance.ChangeState(BuyState.None);
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

        UpdateDailyInfo();
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
        notifyObserver(EventState.EventIcon);
    }

    public void OnClickEventNotify()
    {
        if (BuildingSpawner.instance.GetEventBuildingCount() <= 0) return;

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
        EventManager.instance.RandomRoulette();
        eventRoulette.OnButtonClick();
    }

    public void OnClickSlotMachineButton()
    {
        if (EventManager.instance.CheckEventCondition())
        {
            EventManager.instance.CheckEvents();
            eventRoulette.SetPossibleEvent(EventManager.instance.possibleEvents.ToArray());
            notifyObserver(EventState.SlotMachine);
        }
    }

    public void OnClickDeptDocButton()
    {
        if (RoutineManager.instance.debt != 0)
            notifyObserver(EventState.DebtDoc);
    }

    public void OnClickSpaceBar()
    {
        if (eventRoulette.gameObject.activeSelf)
        {
            OnClickEventRoulette();
        }
        else
            OnClickNextDay();
    }

    #endregion


    private int GetBuildingIndex()
    {
        int idx = targetBuilding.type == BuildingType.Residential ? 0 : 1;
        return idx;
    }

    #region Observer
    public void addObserver(IObserver observer)
    {
        observers.Add(observer);
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

        addObserver(cityLevelUpPanel);
        addObserver(eventNotify);
        addObserver(eventRoulette);

        addObserver(costInfo);
        addObserver(construct);
        addObserver(etcFunc);

        addObserver(debtDoc);
        addObserver(receipt);
        addObserver(reminder);
        addObserver(lateReceipt);

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