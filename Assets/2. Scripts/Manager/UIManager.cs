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

    public InputManager inputManager;

    [Header("Info")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI debtDayText;
    public TextMeshProUGUI dayOfWeekText;
    public TextMeshProUGUI debtInfoText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI chipText;
    public EventInfoUI eventInfo;

    [Header("Building")]
    public BuildingIntroUI[] buildingIntros;
    public InfoUI[] infos;
    public ListUI[] lists;

    [Header("Message")]
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

    [Header("Debt")]
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

    [Header("Tutorial")]
    public TutorialUI tutorial;

    [Header("Minigame")]
    public MinigameUI[] minigames;

    [Header("TimeLapse")]
    public SkipTimeLapseUI skipTimeLapse;

    #endregion

    private Building targetBuilding;
    private List<IObserver> observers = new List<IObserver>();
    private string[] weekStr = { "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일" };

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
        InitObserver();
        notifyObserver(EventState.LockButton);
        InitInfoUI();
    }

    #region SetValue

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
    }

    public void InitInfoUI()
    {
        UpdateDailyInfo();
        Setmoney();
        SetEventInfo(EventManager.instance.curEvents.ToArray());

        if (GameManager.instance.curMapType == MapType.Totopia)
            SetChip();
    }

    public void UpdateDailyInfo()
    {
        DateTime curDay = RoutineManager.instance.day;
        dayText.text = curDay.ToString("yy.MM.dd");
        dayOfWeekText.text = weekStr[(int)curDay.DayOfWeek];
        SetDebt();
        SetDebtInfo();
        SetDDay(curDay);
    }

    public void Setmoney()
    {
        moneyText.text = GetCommaText(ShopManager.instance.Money) + "원";
    }

    public void SetChip()
    {
        chipText.text = GetCommaText(ChipManager.instance.CurChip) + "개";
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
            debtDayText.text = "<size=200>D-" + dDay.ToString() + "</size>";
        else
            debtDayText.text = "<size=155>납부 완료</size>";

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
        TemporayUI message = PoolSystem.instance.messagePool.GetFromPool<TemporayUI>("Error");
        message.SetUI(str, position);
        AkSoundEngine.PostEvent("Play_Deny_01", gameObject);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        string[] str = { amount < 0 ? "<sprite=1> <sprite=5>" : "<sprite=3> <sprite=6>" };

        TemporayUI message = PoolSystem.instance.messagePool.GetFromPool<TemporayUI>("Happiness");
        message.SetUI(str, position);
    }

    public void SetEventTempPopUp(Event curEvent, Vector3 position)
    {
        TemporayUI message = PoolSystem.instance.messagePool.GetFromPool<TemporayUI>("Event");
        message.SetUI(curEvent, position);

        if (curEvent.rewardType == RewardType.Penalty)
            AkSoundEngine.PostEvent("Play_POPUP_Negative", gameObject);
        else if (curEvent.rewardType == RewardType.Reward)
            AkSoundEngine.PostEvent("Play_POPUP_Positive", gameObject);
    }

    public void SetEventIconPopUp(Event curEvent, Vector3 position)
    {
        TemporayUI message = PoolSystem.instance.messagePool.GetFromPool<TemporayUI>("EventIcon");
        message.SetUI(curEvent.eventIcon, position);
    }

    public TemporayUI SetEventHighlightPopUp(Event[] curEvents, Vector3 position)
    {
        TemporayUI message = PoolSystem.instance.messagePool.GetFromPool<TemporayUI>("EventHighlight");
        message.SetUI(curEvents, position);
        return message;
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

    public void SetTutorialPopup(bool isForce = false)
    {
        if (GameManager.instance.skipTutorial && !isForce) return;

        tutorial.isInit = !isForce;
        notifyObserver(EventState.Tutorial);
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

    public void OnClickBuildingBuy(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuyBuilding, index);
    }

    public void OnClickBuildingSell()
    {
        ShopManager.instance.ChangeState(BuyState.SellBuilding);
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
        notifyObserver(EventState.None);
        StartCoroutine(RoutineManager.instance.DailyUpdate());

        if (GameManager.instance.curMapType == MapType.Utopia)
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
            string[] str = { "사회현상이 발생한 건물이 없습니다." };
            SetErrorPopUp(str, transform.position);
            return;
        }
        notifyObserver(EventState.EventIcon);
    }

    public void OnClickEventNotify()
    {
        if (BuildingSpawner.instance.GetProblemBuildingCount() <= 0) 
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

    public void OnClickTutorialButton()
    {
        SetTutorialPopup(true);
    }

    public void OnClickLobbyButton()
    {
        GameManager.instance.LoadLobbyScene();
    }

    public void OnClickQuitGame()
    {
        GameManager.instance.QuitGame();
    }

    public void OnClickMinigame()
    {
        topPanel.SetActive(false);
        bottomPanel.SetActive(false);

        EnterBuilding enterBuilding = ShopManager.instance.GetPickBuilding().GetComponent<EnterBuilding>();
        minigames[(int)enterBuilding.minigameType].InitGame(enterBuilding);
        notifyObserver(EventState.Minigame);
    }

    public void OnClickSound(string name)
    {
        SoundManager.instance.PostEvent(name);
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

        if (GameManager.instance.curMapType == MapType.Totopia && targetBuilding.type == BuildingType.Culture)
            idx = 2;

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

        addObserver(eventNotify);

        addObserver(costInfo);
        addObserver(construct);
        addObserver(etcFunc);

        addObserver(menu);
        addObserver(setting);

        addObserver(destroyMessage);
        addObserver(phone);
        addObserver(gamePhone);

        addObserver(tutorial);

        foreach (UILockButton lockButton in lockButtons)
        {
            addObserver(lockButton);
        }

        if(GameManager.instance.curMapType == MapType.Totopia)
        {
            foreach (MinigameUI game in minigames)
            {
                addObserver(game);
            }
        }
    }
    #endregion
}