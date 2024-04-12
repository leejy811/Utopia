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
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dayText;
    public SolutionUIInfo[] solutionUIInfos;
}

public class BuildingIntroUI : MonoBehaviour
{
    [Header("Building")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingInfoText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI happinessStringText;

    [Header("Special")]
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI utilityText;
    public TextMeshProUGUI userText;
    public TextMeshProUGUI residentText;
    public TextMeshProUGUI[] castText;
    public Button[] optionButtons;
    public Toggle[] optionToggles;

    [Header("Event")]
    public EventUIInfo[] eventUIInfos;

    string[] typeString = { "주거", "상업", "문화", "서비스" };
    string[] subTypeString = { "아파트", "잡화", "영화", "경찰", "음식", "미술", "소방", "여가" };
    string[] valueTypeString = { "상품 가격", "입장료", "취업률" };

    public void SetValue(Building building)
    {
        buildingNameText.text = building.buildingName + building.count;
        buildingInfoText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType] + "/" + building.grade + "등급";
        happinessText.text = "<sprite=" + building.happinessRate / 20 + "> " + building.happinessRate + "(" + building.happinessDifference + ")%";
        happinessStringText.text = building.happinessDifference > 0 ? "행복도 증가<sprite=6>" : building.happinessDifference < 0 ? "행복도 감소<sprite=5>" : "행복도 유지";

        if (building.type == BuildingType.Residential)
        {
            residentText.text = building.values[ValueType.Resident].cur.ToString() + "/"+ building.values[ValueType.Resident].max.ToString();

            for (int i = 0; i < 3; i++)
            {
                castText[i].text = building.values[(ValueType)(i + 1)].BoundaryToString();
            }

            ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
            for (int i = 0; i < residentialBuilding.existFacility.Length; i++)
            {
                if (residentialBuilding.CheckFacility((OptionType)i))
                {
                    SetOptionBuy(residentialBuilding, i);
                }
            }
        }
        else
        {
            influenceText.text = building.influencePower.ToString();
            utilityText.text = valueTypeString[(int)building.type - 1] + ": " + building.values[ValueType.utility].BoundaryToString();
            userText.text = building.values[ValueType.user].BoundaryToString();
        }

        List<Event> curEvent = building.GetEventProblem();

        for (int i = 0;i < eventUIInfos.Length; i++)
        {
            eventUIInfos[i].iconImage.sprite = curEvent[i].eventIcon;
            eventUIInfos[i].nameText.text = curEvent[i].eventName.ToString();
            eventUIInfos[i].dayText.text = "(D-" + (curEvent[i].effectValue.Count - curEvent[i].curDay + 1).ToString() + ")";

            for (int j = 0; j < eventUIInfos[i].solutionUIInfos.Length; j++)
            {
                eventUIInfos[i].solutionUIInfos[j].nameText.text = curEvent[i].solutions[j].name.ToString();
                eventUIInfos[i].solutionUIInfos[j].costText.text = "(-" + curEvent[i].solutions[j].cost.ToString() + "원)";
                eventUIInfos[i].solutionUIInfos[j].probText.text = "해결확률 " + curEvent[i].solutions[j].prob.ToString() + "%";
            }
        }
    }

    public void OnUI(Building building, Vector3 pos)
    {
        transform.localPosition = pos;
        SetValue(building);
    }

    public void SetOptionBuy(ResidentialBuilding residentialBuilding, int index)
    {
        if (residentialBuilding.CheckFacility((OptionType)index))
        {
            ColorBlock block = optionButtons[index].colors;
            block.disabledColor = block.selectedColor;
            optionButtons[index].colors = block;
            optionToggles[index].colors = block;
        }
        else
        {
            ColorBlock block = optionButtons[index].colors;
            block.disabledColor = block.normalColor;
            optionButtons[index].colors = block;
            optionToggles[index].colors = block;
        }

        optionButtons[index].interactable = false;
    }
}
