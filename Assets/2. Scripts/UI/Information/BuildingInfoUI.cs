using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : InfoUI
{
    [Header("Building")]
    public TextMeshProUGUI buildingGradeText;
    public TextMeshProUGUI buildingCostText;

    protected Building building;

    string[] typeString = { "주거", "상업", "문화", "서비스" };
    string[] subTypeString = { "아파트", "잡화", "영화", "경찰", "음식", "미술", "소방", "여가" };

    public override void SetValue(int index)
    {
        building = BuildingSpawner.instance.buildingPrefabs[index].GetComponent<Building>();

        if (!CityLevelManager.instance.CheckBuildingLevel(building.GetComponent<Building>()) && index != 0) return;

        gameObject.SetActive(!gameObject.activeSelf);

        nameText.text = building.buildingName;
        typeText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType];
        buildingGradeText.text = building.grade + "등급";
        buildingCostText.text = building.cost.ToString();
    }
}