using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListUI : ListUI
{
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

                    costText[j].text = building.cost.ToString("C");

                    if (buildingCount[i] + j == 0) continue;

                    bool checkGrade = CityLevelManager.instance.CheckBuildingLevel(building);
                    ButtonImage[j].sprite = !checkGrade ? lockSprite : building.buildingIcon;
                    Button[j].interactable = checkGrade;
                }
                break;
            }
        }
    }
}