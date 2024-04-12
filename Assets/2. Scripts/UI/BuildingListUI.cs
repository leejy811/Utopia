using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingListUI : MonoBehaviour
{
    [Header("Cost")]
    public TextMeshProUGUI[] costText;

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
                    costText[j].text = BuildingSpawner.instance.buildingPrefabs[cnt + j].GetComponent<Building>().cost.ToString() + "$";
                }
                break;
            }
            cnt += buildingCount[i];
        }
    }
}