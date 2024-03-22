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

    [Header("Building")]
    public BuildingUIInfo[] buildingUIInfos;

    [Header("PopUp")]
    public GameObject optionPopUp;
    public GameObject[] buildingPopUp;
    public GameObject[] buildingBuyPopUp;

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
    }

    private void Update()
    {
        Setmoney();
    }

    #region SetValue

    public void UpdateDailyInfo()
    {
        dayText.text = RoutineManager.instance.day.ToString();
        moneyText.text = ShopManager.instance.money.ToString();
        happinessText.text = RoutineManager.instance.cityHappiness.ToString();
    }

    public void Setmoney()
    {
        moneyText.text = ShopManager.instance.money.ToString();
    }

    public void SetBuildingValue()
    {
        BuildingUIInfo info = buildingUIInfos[targetBuilding.curEvents.Count];

        info.nameText.text = targetBuilding.subType.ToString() + " " + BuildingSpawner.instance.buildingCount[(int)targetBuilding.subType + 1];
        info.gradeText.text = targetBuilding.grade.ToString();
        info.typeText.text = targetBuilding.type.ToString();
        info.subTypeText.text = targetBuilding.subType.ToString();
        info.happinessText.text = targetBuilding.happinessRate.ToString() + "%";

        for(int i = 0;i < info.eventUIInfos.Length; i++)
        {
            info.eventUIInfos[i].nameText.text = targetBuilding.curEvents[i].eventEngName.ToString();
            info.eventUIInfos[i].dayText.text = (targetBuilding.curEvents[i].effectValue.Count - targetBuilding.curEvents[i].curDay).ToString();

            for(int j = 0;j < info.eventUIInfos[i].solutionUIInfos.Length; j++)
            {
                info.eventUIInfos[i].solutionUIInfos[j].nameText.text = targetBuilding.curEvents[i].solutions[j].engName.ToString();
                info.eventUIInfos[i].solutionUIInfos[j].costText.text = "(-" + targetBuilding.curEvents[i].solutions[j].cost.ToString() + ")";
                info.eventUIInfos[i].solutionUIInfos[j].probText.text = targetBuilding.curEvents[i].solutions[j].prob.ToString() + "%";
            }
        }
    }

    #endregion

    #region PopUp

    public void SetOptionPopUp(bool active)
    {
        optionPopUp.SetActive(active);
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
            return;
        }

        targetBuilding = building.GetComponent<Building>();

        for(int i = 0;i < buildingPopUp.Length; i++)
        {
            if(i == targetBuilding.curEvents.Count)
                buildingPopUp[i].SetActive(active);
            else
                buildingPopUp[i].SetActive(false);
        }

        SetBuildingValue();
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

    public void OnClickTileBuild(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuildTile, index);
    }

    public void OnClickOptionBuy(int index)
    {
        ShopManager.instance.BuyOption((OptionType)index);
    }

    public void OnClickNextDay()
    {
        RoutineManager.instance.DailyUpdate();
    }

    public void OnClickSolveEvent(int index)
    {
        ShopManager.instance.SolveEvent(index);
    }

    #endregion
}
