using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListUI : ListUI
{
    int[] buildingCount = { 0, 3, 6, 9 };

    public override void SetValue(int type)
    {
        for(int i = 0;i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            if (type == i)
            {
                for(int j = 0;j < costText.Length;j++)
                {
                    Building building = BuildingSpawner.instance.buildingPrefabs[buildingCount[i] + j].GetComponent<Building>();
                    costText[j].text = ShopManager.instance.CalculateBuildingCost(building, buildingCount[i] + j).ToString("C");

                    bool checkGrade = CityLevelManager.instance.CheckBuildingLevel(building);
                    ButtonImage[j].sprite = !checkGrade ? lockSprite : building.buildingIcon;
                    Button[j].interactable = checkGrade;
                }
                break;
            }
        }
    }
}