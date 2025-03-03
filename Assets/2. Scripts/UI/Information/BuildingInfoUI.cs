using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : InfoUI
{
    [Header("Building")]
    public TextMeshProUGUI buildingCostText;

    protected Building building;

    protected string[] typeString = { "주거", "상업", "문화", "서비스" };
    protected string[] subTypeString = { "아파트", "잡화", "영화", "경찰", "음식", "미술", "소방", "여가", "의료", "오락" };

    public override void SetValue(int index)
    {
        building = BuildingSpawner.instance.buildingPrefabs[index].GetComponent<Building>();

        if (!CityLevelManager.instance.CheckBuildingLevel(building.GetComponent<Building>()) && index != 0) return;

        gameObject.SetActive(!gameObject.activeSelf);
        nameText.text = building.buildingName;
        typeText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType];
        buildingCostText.text = ShopManager.instance.CalculateBuildingCost(building, index).ToString("#,##0") + "원";
    }
}