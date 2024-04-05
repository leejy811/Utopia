using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SolutionUIInfo
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI probText;
}

[Serializable]
public struct EventUIInfo
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dayText;
    public SolutionUIInfo[] solutionUIInfos;
}

[Serializable]
public struct BuildingUIInfo
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI subTypeText;
    public TextMeshProUGUI happinessText;
    public EventUIInfo[] eventUIInfos;
}

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
    public BuildingUIInfo[] buildingUIInfos;

    [Header("Building Special")]
    public TextMeshProUGUI residentText;
    public TextMeshProUGUI commercialText;
    public TextMeshProUGUI cultureText;
    public TextMeshProUGUI serviceText;

    public TextMeshProUGUI[] faciltyTexts;
    public TextMeshProUGUI[] faciltyBuyTexts;

    public TextMeshProUGUI buildingInfluenceText;
    public TextMeshProUGUI costText;

    [Header("Roulette")]
    public TextMeshProUGUI rouletteText;

    [Header("Influence")]
    public TextMeshProUGUI[] tileInfluenceText;

    [Header("PopUp")]
    public GameObject optionPopUp;
    public GameObject roulettePopUp;
    public GameObject influencePopUp;
    public GameObject[] buildingPopUp;
    public GameObject[] buildingBuyPopUp;
    public GameObject[] buildingSpecialPopUp;
    public GameObject[] ButtonInfoPopUp;

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
        //UpdateDailyInfo();

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
        SetCityResident();
    }

    public void SetHappiness()
    {
        happinessText.text = ((int)RoutineManager.instance.cityHappiness).ToString();
    }

    public void Setmoney()
    {
        //moneyText.text = GetCommaText(ShopManager.instance.Money);
    }

    public void SetCityResident()
    {
        cityResidentText.text = GetCommaText(ResidentialBuilding.cityResident);
    }

    public void SetBuildingValue()
    {
        List<Event> curEvent = targetBuilding.GetEventProblem();
        BuildingUIInfo info = buildingUIInfos[curEvent.Count];

        info.nameText.text = targetBuilding.subType.ToString() + " " + targetBuilding.count;
        info.gradeText.text = targetBuilding.grade.ToString();
        info.typeText.text = targetBuilding.type.ToString();
        info.subTypeText.text = targetBuilding.subType.ToString();
        info.happinessText.text = targetBuilding.happinessRate.ToString() + "%";

        for(int i = 0;i < info.eventUIInfos.Length; i++)
        {
            info.eventUIInfos[i].nameText.text = curEvent[i].eventEngName.ToString();
            info.eventUIInfos[i].dayText.text = (curEvent[i].effectValue.Count - curEvent[i].curDay).ToString();

            for(int j = 0;j < info.eventUIInfos[i].solutionUIInfos.Length; j++)
            {
                info.eventUIInfos[i].solutionUIInfos[j].nameText.text = curEvent[i].solutions[j].engName.ToString();
                info.eventUIInfos[i].solutionUIInfos[j].costText.text = "(-" + curEvent[i].solutions[j].cost.ToString() + ")";
                info.eventUIInfos[i].solutionUIInfos[j].probText.text = curEvent[i].solutions[j].prob.ToString() + "%";
            }
        }

        if(targetBuilding.type == BuildingType.Residential)
        {
            buildingSpecialPopUp[0].SetActive(true);
            buildingSpecialPopUp[1].SetActive(false);

            ResidentialBuilding residential = targetBuilding as ResidentialBuilding;

            residentText.text = targetBuilding.values[ValueType.Resident].cur.ToString() + " / " + targetBuilding.values[ValueType.Resident].max.ToString();
            commercialText.text = targetBuilding.values[ValueType.CommercialCSAT].cur.ToString();
            cultureText.text = targetBuilding.values[ValueType.CultureCSAT].cur.ToString();
            serviceText.text = targetBuilding.values[ValueType.ServiceCSAT].cur.ToString();

            for(int i = 0;i < faciltyTexts.Length;i++)
            {
                faciltyTexts[i].text = residential.CheckFacility((OptionType)i) ? "O" : "X";
            }
        }
        else
        {
            buildingSpecialPopUp[1].SetActive(true);
            buildingSpecialPopUp[0].SetActive(false);
            buildingInfluenceText.text = targetBuilding.influencePower.ToString();

            if(targetBuilding.type == BuildingType.Service)
            {
                buildingSpecialPopUp[2].SetActive(true);
                costText.text = (targetBuilding as ServiceBuilding).costPerDay.ToString();
            }
            else
                buildingSpecialPopUp[2].SetActive(false);
        }
    }

    public void SetOption(GameObject building)
    {
        for(int i = 0;i < System.Enum.GetValues(typeof(OptionType)).Length; i++)
        {
            bool exist = building.GetComponent<ResidentialBuilding>().CheckFacility((OptionType)i);
            if(exist)
                faciltyBuyTexts[i].text = "Complete";
            else
                faciltyBuyTexts[i].text = "Add(-500)";
        }
    }

    public void SetRoulette(List<Event> ranEvents)
    {
        rouletteText.text = ranEvents[0].eventEngName + " / " + ranEvents[1].eventEngName + " / " + ranEvents[2].eventEngName;
    }

    #endregion

    #region PopUp

    public void SetOptionPopUp(bool active, GameObject building = null)
    {
        optionPopUp.SetActive(active);

        if(building != null)
        {
            SetOption(building);
        }
    }

    public void SetRoulettePopUp(bool active)
    {
        roulettePopUp.SetActive(active);
    }

    public void SetBuildingPopUp(bool active, GameObject building = null)
    {
        if(building == null)
        {
            targetBuilding = null;
            for (int i = 0; i < buildingPopUp.Length; i++)
            {
                buildingPopUp[i].SetActive(false);
            }
            buildingSpecialPopUp[0].SetActive(false);
            buildingSpecialPopUp[1].SetActive(false);
            return;
        }

        targetBuilding = building.GetComponent<Building>();

        for(int i = 0;i < buildingPopUp.Length; i++)
        {
            if(i == targetBuilding.GetEventProblem().Count)
                buildingPopUp[i].SetActive(active);
            else
                buildingPopUp[i].SetActive(false);
        }

        SetBuildingValue();
    }

    public void SetInfluencePopUp(Tile tile = null)
    {
        if (tile == null)
            influencePopUp.SetActive(false);
        else
        {
            influencePopUp.SetActive(true);
            for (int i = 0; i < tileInfluenceText.Length; i++)
            {
                tileInfluenceText[i].text = tile.influenceValues[i].ToString();
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            Camera uiCamera = canvas.worldCamera;
            RectTransform rectParent = canvas.GetComponent<RectTransform>();
            RectTransform rectSelf = influencePopUp.GetComponent<RectTransform>();

            var screenPos = Camera.main.WorldToScreenPoint(tile.transform.position);

            var localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

            rectSelf.localPosition = localPos;
        }
    }

    public void SetButtonInfoPopUp(int index, RectTransform rect)
    {
        for (int i = 0; i < ButtonInfoPopUp.Length; i++)
        {
            if (i == index)
            {
                ButtonInfoPopUp[i].SetActive(!ButtonInfoPopUp[i].activeSelf);
                ButtonInfoPopUp[i].GetComponent<RectTransform>().localPosition = rect.localPosition;
                return;
            }
        }
    }

    #endregion

    #region OnClick

    public void OnClickBuildingBuyPopUp(int index)
    {
        for (int i = 0;i < buildingBuyPopUp.Length;i++)
        {
            if(i == index)
                buildingBuyPopUp[i].SetActive(!buildingBuyPopUp[i].activeSelf);
            else
                buildingBuyPopUp[i].SetActive(false);
        }
        ShopManager.instance.ChangeState(BuyState.None);
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

    public void OnClickTileBuild(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuildTile, index);
    }

    public void OnClickOptionBuy(int index)
    {
        if (ShopManager.instance.BuyOption((OptionType)index))
            faciltyBuyTexts[index].text = "Complete";
    }

    public void OnClickNextDay()
    {
        OnClickBuildingBuyPopUp(-1);
        RoutineManager.instance.DailyUpdate();
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

    #endregion
}
