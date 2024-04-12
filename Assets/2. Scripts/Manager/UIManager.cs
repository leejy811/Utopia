using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    #region UIComponent

    public Canvas canvas;

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

    [Header("Buttons")]
    public Button[] buttons;

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
        LockButtons(false);
    }

    #region SetValue
    
    public void LockButtons(bool active)
    {
        foreach (Button button in buttons)
        {
            button.interactable = active;
        }

        if(active)
            SetCityLevelPopUp(true);
    }

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

    public void SetBuildingInfoPopUp(int typeIndex, int buildingIndex, float xPos)
    {
        if(typeIndex == (int)UIButtonType.Tile)
        {
            SetTileInfoPopUp(buildingIndex, xPos);
        }

        for (int i = 0; i < buildingInfos.Length; i++)
        {
            if (i == typeIndex)
            {
                buildingInfos[i].gameObject.SetActive(!buildingInfos[i].gameObject.activeSelf);

                Vector3 pos = buildingInfos[i].gameObject.GetComponent<RectTransform>().localPosition;
                buildingInfos[i].OnUI(BuildingSpawner.instance.buildingPrefabs[buildingIndex].GetComponent<Building>(), new Vector3(xPos, pos.y, pos.z));
                return;
            }
        }
    }

    public void SetBuildingIntroPopUp(Building building = null)
    {
        if(eventNotify.gameObject.activeSelf)
            SetEventNotifyValue(building);

        if (building == null)
        {
            foreach(BuildingIntroUI introUI in buildingIntros)
                introUI.gameObject.SetActive(false);
        }
        else
        {
            targetBuilding = building;
            int idx = GetBuildingIndex();

            for(int i = 0;i < buildingIntros.Length;i++)
            {
                if(i == idx)
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

    public void SetTileInfoPopUp(int tileIndex, float xPos)
    {
        tileInfo.gameObject.SetActive(!tileInfo.gameObject.activeSelf);

        Vector3 pos = tileInfo.gameObject.GetComponent<RectTransform>().localPosition;
        tileInfo.OnUI(null, new Vector3(xPos, pos.y, pos.z));
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
        TemporayUI message =  Instantiate(errorMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        string massage = amount < 0 ? "<sprite=1> <sprite=5>" : "<sprite=3> <sprite=6>";

        TemporayUI message = Instantiate(happinessMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
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

    public void SetAllPopUp()
    {
        ShopManager.instance.ChangeState(BuyState.None);
        OnClickBuildingBuyPopUp(-1);
        SetEventNotifyPopUp(false);
        SetCityLevelPopUp(false);
        tileListPopUp.gameObject.SetActive(false);
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

        if (index != -1)
            SetBuildingListValue(index);
        else
        {
            for (int i = 0; i < buildingInfos.Length; i++)
                buildingInfos[i].gameObject.SetActive(false);
        }

        ShopManager.instance.ChangeState(BuyState.None);
        SetEventNotifyPopUp(false);
        SetCityLevelPopUp(false);
        tileListPopUp.gameObject.SetActive(false);
    }

    public void OnClickTileBuildPopUp(bool active)
    {
        SetAllPopUp();
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
        SetAllPopUp();
        ShopManager.instance.ChangeState(BuyState.SellBuilding);
    }

    public void OnClickTileBuy()
    {
        SetAllPopUp();
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
        SetAllPopUp();
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
        SetAllPopUp();
        Grid.instance.SetTileColorMode();
    }

    public void OnClickTileInfluenceMode()
    {
        SetAllPopUp();
        Grid.instance.SetTileInfluenceMode();
    }

    public void OnClickCityLevelMode()
    {
        SetAllPopUp();
        SetCityLevelPopUp(true);
    }

    public void OnClickEventHighLight()
    {
        SetAllPopUp();
        BuildingSpawner.instance.EventBuildingsHighlight();
    }

    public void OnClickEventNotify()
    {
        SetAllPopUp();
        ShopManager.instance.ChangeState(BuyState.EventCheck);
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
