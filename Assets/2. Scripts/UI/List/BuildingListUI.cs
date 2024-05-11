using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListUI : ListUI
{
    [Header("Button")]
    public Button[] Button;
    public Sprite lockSprite;

    int[] buildingCount = { 0, 3, 7, 10 };

    public override void SetValue(int type)
    {
        for(int i = 0;i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            if (type == i)
            {
                for(int j = 0;j < costText.Length;j++)
                {
                    Building building = BuildingSpawner.instance.buildingPrefabs[buildingCount[i] + j].GetComponent<Building>();
                    bool checkGrade = CityLevelManager.instance.CheckBuildingLevel(building);
                    costText[j].text = building.cost.ToString() + "$";
                    ButtonImage[j].sprite = !checkGrade ? lockSprite : building.buildingIcon;
                    Button[j].interactable = checkGrade;

                    Debug.Log(building.buildingName);
                }
                break;
            }
        }
    }
}