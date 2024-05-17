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
    public TextMeshProUGUI NewsMessage;
    public TextMeshProUGUI NewsMessage2;

    [Header("Building")]
    public BuildingIntroUI[] buildingIntros;
    public InfoUI[] infos;
    public ListUI[] lists;

    [Header("Tile")]
    public TileInfluenceUI tileInfluenceInfo;

    [Header("Statistic")]
    public StatisticUI statistic;

    [Header("CityLevel")]
    public UIElement cityLevelPanel;
    public CityLevelUI[] cityLevels;

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

    #endregion


    private Building targetBuilding;
    private List<IObserver> observers = new List<IObserver>();

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
        notifyObserver(EventState.LockButton);
    }

    #region NewsMessage

    void Update()
    {
        NewsHappiness = (int)RoutineManager.instance.cityHappiness;

        if (NewsHappiness != 0 && UpdateNews == false)
        {
            previousHappiness = NewsHappiness;
            UpdateNews = true;
        }

        if (previousHappiness != NewsHappiness && UpdateNews == true)
        {
            StartCoroutine(HappinessBasedNews());
        }
    }


    IEnumerator HappinessBasedNews()
    {
        imageRectTransform.gameObject.SetActive(true);
        if (previousHappiness > 20 && NewsHappiness <= 20)
        {
            previousHappiness = NewsHappiness;
            NewsMessage2.text = "이게 도시냐";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "이게 도시냐";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "이 곳도 다른 곳이랑 다를게 없구나...";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "이 곳도 다른 곳이랑 다를게 없구나...";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() => 
            { imageRectTransform.gameObject.SetActive(false); });

        }
        else if (previousHappiness < 20 && NewsHappiness >= 20)
        {

            previousHappiness = NewsHappiness;
            NewsMessage2.text = "시장님 잘 좀 해봐요...";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "시장님 잘 좀 해봐요...";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "이 곳도 다른 곳이랑 다를게 없구나...라고 할뻔";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "이 곳도 다른 곳이랑 다를게 없구나...라고 할뻔";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });

        }
        else if (previousHappiness > 40 && NewsHappiness <= 40)
        {
            previousHappiness = NewsHappiness;
            NewsMessage2.text = "시장님 저희가 뭘 도와드려야 될까요?";

            Canvas.ForceUpdateCanvases(); 
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y); 

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "시장님 저희가 뭘 도와드려야 될까요?";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "우리 시장 틀니 압수";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "우리 시장 틀니 압수";
            // yield return new WaitForSeconds(2);
            //
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });

        }
        else if (previousHappiness < 40 && NewsHappiness >= 40)
        {
            previousHappiness = NewsHappiness;
            NewsMessage2.text = "다행히 도시가 점점 좋아지고 있어";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "다행히 도시가 점점 좋아지고 있어";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "시장님 임플란트 심어드리게요";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "시장님 임플란트 심어드리게요";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });


        }
        else if (previousHappiness > 60 && NewsHappiness <= 60)
        {

            previousHappiness = NewsHappiness;
            NewsMessage2.text = "요즘 도시 운영이 좀 이상하지 않아?";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "요즘 도시 운영이 좀 이상하지 않아?";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "취직이 잘되는 사회를 만들던가";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "취직이 잘되는 사회를 만들던가";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });

        }
        else if (previousHappiness < 60 && NewsHappiness >= 60)
        {

            previousHappiness = NewsHappiness;
            NewsMessage2.text = "시장님 믿고 있었습니다! 요즘 도시가 너무 좋아요!";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "시장님 믿고 있었습니다! 요즘 도시가 너무 좋아요!";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "지금부터 시장님과 나는 한 몸으로 간주한다.";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "지금부터 시장님과 나는 한 몸으로 간주한다.";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });

        }
        else if (previousHappiness > 80 && NewsHappiness <= 80)
        {
            previousHappiness = NewsHappiness;
            NewsMessage2.text = "조금만 더...화이팅입니다!";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "조금만 더...화이팅입니다!";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "뭐 조금 아쉬운거지~";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "뭐 조금 아쉬운거지~";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });
        }
        else if (previousHappiness < 80 && NewsHappiness >= 80)
        {
            previousHappiness = NewsHappiness;
            NewsMessage2.text = "이곳이 유토피아라는게 학계의 정설";

            Canvas.ForceUpdateCanvases();
            float preferredWidth = NewsMessage2.preferredWidth;

            Vector2 newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            {
                NewsMessage.text = "이곳이 유토피아라는게 학계의 정설";
            });
            yield return new WaitForSeconds(2);


            NewsMessage.text = " ";
            // NewsMessage2.text = "도시 역사상 최고...GOAT";
            //
            // yield return new WaitForSeconds(1);
            // Canvas.ForceUpdateCanvases();
            //
            // preferredWidth = NewsMessage2.preferredWidth;
            //
            // newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);
            //
            // imageRectTransform.DOSizeDelta(newSize, animationDuration);
            //
            // NewsMessage.text = "도시 역사상 최고...GOAT";
            // yield return new WaitForSeconds(2);
            // NewsMessage.text = " ";

            preferredWidth = NewsMessage.preferredWidth;

            newSize = new Vector2(preferredWidth + 20, imageRectTransform.sizeDelta.y);

            imageRectTransform.DOSizeDelta(newSize, animationDuration).OnComplete(() =>
            { imageRectTransform.gameObject.SetActive(false); });
        }
        else
            imageRectTransform.gameObject.SetActive(false);
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
        dayText.text = curDay.ToString("yy") + "/" + curDay.ToString("MM") + "/" + curDay.ToString("dd");
        SetHappiness();
        Setmoney();
        SetCityResident();
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

    public void SetTileInfluenceValue(Tile tile)
    {
        tileInfluenceInfo.SetValue(tile);
    }

    #endregion

    #region PopUp

    public void SetRoulettePopUp(bool active, Event[] ranEvents = null)
    {
        eventRoulette.gameObject.SetActive(active);

        if (active)
            eventRoulette.SetEvent(ranEvents);
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

    public void SetTileInfluencePopUp(Tile tile = null)
    {
        if (tile == null)
            tileInfluenceInfo.gameObject.SetActive(false);
        else
        {
            tileInfluenceInfo.gameObject.SetActive(true);
            tileInfluenceInfo.SetValue(tile);
        }
    }

    public void SetStatisticPopUp(bool active)
    {
        statistic.gameObject.SetActive(active);
        statistic.SetValue();
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

    public void SetCityLevelPopUp(bool active)
    {
        cityLevelPanel.gameObject.SetActive(active);

        if (active)
        {
            foreach(CityLevelUI cityLevel in cityLevels)
            {
                cityLevel.SetValue();
            }
        }
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

        RoutineManager.instance.DailyUpdate();
        notifyObserver(EventState.Statistic);
    }

    public void OnClickCloseStatistic()
    {
        SetStatisticPopUp(false);
        RoutineManager.instance.UpdateAfterStat();
        UpdateDailyInfo();
    }

    public void OnClickCloseEventRoulette()
    {
        SetRoulettePopUp(false);
    }

    public void OnClickSolveEvent(int index)
    {
        ShopManager.instance.SolveEvent(index);
    }

    public void OnClickTileColorMode()
    {
        notifyObserver(EventState.TileColor);
    }

    public void OnClickTileInfluenceMode()
    {
        notifyObserver(EventState.TileInfluence);
    }

    public void OnClickCityLevelMode()
    {
        notifyObserver(EventState.CityLevel);
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
        eventRoulette.OnButtonClick();
    }

    public void OnClickSpaceBar()
    {
        if (CityLevelManager.instance.levelIdx == -1) return;

        if (statistic.gameObject.activeSelf)
        {
            if (statistic.doStamp)
            {
                OnClickCloseStatistic();
                statistic.doStamp = false;
                return;
            }

            statistic.gameObject.GetComponentInChildren<Animator>().SetTrigger("DoStamp");
            statistic.doStamp = true;
        }
        else if (eventRoulette.gameObject.activeSelf)
        {
            if (eventRoulette.state == RouletteState.End)
                OnClickCloseEventRoulette();
            else if (eventRoulette.state == RouletteState.Start)
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
        addObserver(statistic);

        addObserver(cityLevelPanel);
        addObserver(cityLevelUpPanel);
        addObserver(eventNotify);

        addObserver(costInfo);
        addObserver(construct);
        addObserver(etcFunc);

        foreach(CityLevelUI cityLevel in cityLevels)
        {
            addObserver(cityLevel);
        }

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