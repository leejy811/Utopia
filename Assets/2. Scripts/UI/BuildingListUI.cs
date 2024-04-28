using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListUI : MonoBehaviour
{
    [Header("Cost")]
    public TextMeshProUGUI[] costText;

    [Header("Button")]
    public Button[] buildingButton;
    public Image[] buildingButtonImage;
    public Sprite lockSprite;

    int[] buildingCount = { 3, 4, 2, 3 };

    public void SetValue(BuildingType type)
    {
        int cnt = 0;

        for(int i = 0;i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            if(type == (BuildingType)i)
            {
                for(int j = 0;j < costText.Length;j++)
                {
                    Building building = BuildingSpawner.instance.buildingPrefabs[cnt + j].GetComponent<Building>();
                    bool checkGrade = CityLevelManager.instance.CheckBuildingLevel(building);
                    costText[j].text = building.cost.ToString() + "$";
                    buildingButtonImage[j].sprite = !checkGrade ? lockSprite : building.buildingIcon;
                    buildingButton[j].interactable = checkGrade;
                }
                break;
            }
            cnt += buildingCount[i];
        }
    }
}