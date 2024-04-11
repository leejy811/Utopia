using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    #region UIComponent

    [Header("Info")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI cityResidentText;

    [Header("Building")]
    public BuildingInfoUI[] buildingInfos;
    public BuildingIntroUI[] buildingIntros;
    public BuildingListUI[] buildingLists;

    [Header("Tile")]
    public TileInfoUI tileInfo;
    public TileInfluenceUI tileInfluenceInfo;
    public GameObject tileListPopUp;

    [Header("Statistic")]
    public StatisticUI statistic;

    [Header("CityLevel")]
    public CityLevelUI cityLevel;

    [Header("Roulette")]
    public EventRouletteUI eventRoulette;

    [Header("Message")]
    public GameObject errorMessagePrefab;
    public GameObject happinessMessagePrefab;

    [Header("Cost")]
    public CostUI costInfo;

    [Header("Event Notify")]
    public EventNotifyUI eventNotify;

    #endregion

    private Building targetBuilding;

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
        UpdateDailyInfo();

        //To Do 게임 시작 후 주거 건물 건설만 가능하도록
    }

    #region SetValue

    private string GetCommaText(int data)
    {
        return string.Format("{0:#,###}", data);
    }

    public void UpdateDailyInfo()
    {
        DateTime curDay = RoutineManager.instance.day;
        dayText.text = curDay.ToString("yy") + "/" + curDay.ToString("MM") + "/" + curDay.ToString("dd");
        SetHappiness();
        Setmoney();
        //SetCityResident();
    }

    public void SetHappiness()
    {
        happinessText.text = ((int)RoutineManager.instance.cityHappiness).ToString();
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
        buildingLists[index].SetValue((BuildingType)index);
    }

    public void SetTileInfoValue()
    {
        tileInfo.SetValue(Grid.instance.tilePrefab.GetComponent<Tile>());
    }

    public void SetTileInfluenceValue(Tile tile)
    {
        tileInfluenceInfo.SetValue(tile);
    }

    #endregion

    #region PopUp

    public void SetRoulettePopUp(bool active, List<Event> ranEvents = null)
    {
        eventRoulette.gameObject.SetActive(active);

        if (active)
            eventRoulette.SetValue(ranEvents);
    }

    public void SetBuildingInfoPopUp(int typeIndex, int buildingIndex, RectTransform rect)
    {
        if(typeIndex == (int)UIButtonType.Tile)
        {
            SetTileInfoPopUp(buildingIndex, rect);
        }

        for (int i = 0; i < buildingInfos.Length; i++)
        {
            if (i == typeIndex)
            {
                buildingInfos[i].gameObject.SetActive(!buildingInfos[i].gameObject.activeSelf);

                Vector3 pos = buildingInfos[i].gameObject.GetComponent<RectTransform>().localPosition;
                float xOffset = 0.0f;
                buildingInfos[i].OnUI(BuildingSpawner.instance.buildingPrefabs[buildingIndex].GetComponent<Building>(), new Vector3(rect.position.x + xOffset, pos.y, pos.z));
                return;
            }
        }
    }

    public void SetBuildingIntroPopUp(Building building = null)
    {
        if(eventNotify.gameObject.activeSelf)
            SetEventNotifyValue(building);

        targetBuilding = building;
        int idx = GetBuildingIndex();

        if (building == null)
            buildingIntros[idx].gameObject.SetActive(false);
        else
        {
            buildingIntros[idx].gameObject.SetActive(true);
            buildingIntros[idx].SetValue(building);
        }
    }

    public void SetEventNotifyPopUp(bool active)
    {
        eventNotify.gameObject.SetActive(active);

        if (active)
            eventNotify.Init();
    }

    public void SetTileInfoPopUp(int tileIndex, RectTransform rect)
    {
        tileInfo.gameObject.SetActive(!tileInfo.gameObject.activeSelf);

        Vector3 pos = tileInfo.gameObject.GetComponent<RectTransform>().localPosition;
        float xOffset = 0.0f;
        tileInfo.OnUI(null, new Vector3(rect.position.x + xOffset, pos.y, pos.z));
        return;
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
        TemporayUI message =  Instantiate(errorMessagePrefab).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        string massage = amount > 0 ? "<sprite=?> <sprite=?>" : "<sprite=?> <sprite=?>";

        TemporayUI message = Instantiate(happinessMessagePrefab).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetCostPopUp(int cost, Vector3 position)
    {
        costInfo.OnUI(cost, position);
    }

    public void SetCityLevelPopUp(bool active)
    {
        cityLevel.gameObject.SetActive(active);

        if (active)
            cityLevel.SetValue();
    }

    #endregion

    #region OnClick

    public void OnClickBuildingBuyPopUp(int index)
    {
        for (int i = 0;i < buildingLists.Length;i++)
        {
            if(i == index)
                buildingLists[i].gameObject.SetActive(!buildingLists[i].gameObject.activeSelf);
            else
                buildingLists[i].gameObject.SetActive(false);
        }
        SetBuildingListValue(index);
        ShopManager.instance.ChangeState(BuyState.None);
    }

    public void OnClickTileBuildPopUp(bool active)
    {
        tileListPopUp.gameObject.SetActive(active);
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
        OnClickBuildingBuyPopUp(-1);
        ShopManager.instance.ChangeState(BuyState.SellBuilding);
    }

    public void OnClickTileBuy()
    {
        OnClickBuildingBuyPopUp(-1);
        ShopManager.instance.ChangeState(BuyState.BuyTile);
    }

    public void OnClickOptionBuy(int index)
    {
        if (ShopManager.instance.BuyOption((OptionType)index))
        {
            int idx = GetBuildingIndex();
            buildingIntros[idx].SetValue(targetBuilding);
        }
    }

    public void OnClickNextDay()
    {
        OnClickBuildingBuyPopUp(-1);
        RoutineManager.instance.DailyUpdate();
        SetStatisticPopUp(true);
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
        Grid.instance.SetTileColorMode();
    }

    public void OnClickTileInfluenceMode()
    {
        Grid.instance.SetTileInfluenceMode();
    }

    public void OnClickCityLevelMode()
    {
        SetCityLevelPopUp(true);
    }

    public void OnClickEventHighLight()
    {
        BuildingSpawner.instance.EventBuildingsHighlight();
    }

    public void OnClickEventNotify()
    {
        SetEventNotifyPopUp(true);
    }

    public void OnClickEventNotifyNext(bool isRight)
    {
        if (EventManager.instance.eventBuildings.Count == 0)
            return;

        eventNotify.NextBuilding(isRight);
    }

    #endregion

    private int GetBuildingIndex()
    {
        int idx = targetBuilding.type == BuildingType.Residential ? 0 : 1;
        int eventidx = targetBuilding.curEvents.Count;
        return idx * 3 + eventidx;
    }
}
