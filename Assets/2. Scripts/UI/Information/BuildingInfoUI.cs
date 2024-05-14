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

    string[] typeString = { "�ְ�", "���", "��ȭ", "����" };
    string[] subTypeString = { "����Ʈ", "��ȭ", "��ȭ", "����", "����", "�̼�", "�ҹ�", "����" };

    public override void SetValue(int index)
    {
        building = BuildingSpawner.instance.buildingPrefabs[index].GetComponent<Building>();

        if (!CityLevelManager.instance.CheckBuildingLevel(building.GetComponent<Building>()) && index != 0) return;

        gameObject.SetActive(!gameObject.activeSelf);

        nameText.text = building.buildingName;
        typeText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType];
        buildingGradeText.text = building.grade + "���";
        buildingCostText.text = building.cost.ToString();
    }
}