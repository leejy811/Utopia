using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : MonoBehaviour
{
    [Header("Building")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingTypeText;
    public TextMeshProUGUI buildingGradeText;
    public TextMeshProUGUI buildingCostText;

    [Header("Special")]
    public TextMeshProUGUI residentText;
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI costPerDayText;
    public Button[] optionButtons;
    public Toggle[] optionToggles;

    string[] typeString = { "주거", "상업", "문화", "서비스" };
    string[] subTypeString = { "아파트", "잡화", "영화", "경찰", "음식", "미술", "소방", "여가" };

    public void SetValue(Building building)
    {
        buildingNameText.text = building.buildingName;
        buildingTypeText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType];
        buildingGradeText.text = building.grade + "등급";
        buildingCostText.text = building.cost.ToString();

        if(building.type == BuildingType.Residential)
        {
            ResidentialBuilding residentialBuilding = building as ResidentialBuilding;

            residentText.text = residentialBuilding.residentCnt.max.ToString();

            for (int i = 0;i < residentialBuilding.existFacility.Length; i++)
            {
                if (residentialBuilding.CheckFacility((OptionType)i))
                {
                    ColorBlock block = optionButtons[i].colors;
                    block.disabledColor = block.selectedColor;
                    optionButtons[i].colors = block;
                    optionToggles[i].colors = block;
                }
                else
                {
                    ColorBlock block = optionButtons[i].colors;
                    block.disabledColor = block.normalColor;
                    optionButtons[i].colors = block;
                    optionToggles[i].colors = block;
                }
            }
        }
        else
        {
            influenceText.text = building.influencePower.ToString();

            if(building.type == BuildingType.Service)
                costPerDayText.text = (building as ServiceBuilding).costPerDay.ToString();
        }
    }

    public void OnUI(Building building, Vector3 pos)
    {
        transform.localPosition = pos;
        SetValue(building);
    }
}