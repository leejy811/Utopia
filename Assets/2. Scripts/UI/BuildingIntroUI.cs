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

    public void SetEventUIInfo(Event curEvent)
    {
        iconImage.sprite = curEvent.eventIcon;
        nameText.text = curEvent.eventName.ToString();
        dayText.text = "(D-" + (curEvent.effectValue.Count - curEvent.curDay + 1).ToString() + ")";

        for (int j = 0; j < solutionUIInfos.Length; j++)
        {
            solutionUIInfos[j].nameText.text = curEvent.solutions[j].name.ToString();
            solutionUIInfos[j].costText.text = "(-" + curEvent.solutions[j].cost.ToString() + "원)";
            solutionUIInfos[j].probText.text = "해결확률 " + curEvent.solutions[j].prob.ToString() + "%";
        }
    }
}

[Serializable]
public struct ValueSlider
{
    public TextMeshProUGUI minText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI curText;
    public TextMeshProUGUI stringText;
    public Slider slider;

    public void SetSlider(BoundaryValue value)
    {
        minText.text = value.min.ToString();
        maxText.text = value.max.ToString();
        curText.text = value.cur.ToString();
        stringText.text = value.CastToString();

        int boundary = (int)value.CheckBoundary();
        slider.value = (float)(boundary + 2) / 3;
    }
}

public class BuildingIntroUI : MonoBehaviour
{
    [Header("Building")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingInfoText;
    public TextMeshProUGUI happinessText;

    [Header("Special")]
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI residentText;
    public TextMeshProUGUI valueTypeText;
    public ValueSlider utilitySlider;
    public ValueSlider userSlider;
    public ValueSlider[] castSlider;
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
        happinessText.text = "<sprite=" + Mathf.Min(building.happinessRate / 20, 4) + "> " + building.happinessRate + "(" + GetSignString(building.happinessDifference, "+") + ")%";

        if (building.type == BuildingType.Residential)
        {
            residentText.text = building.values[ValueType.Resident].cur.ToString() + "/"+ building.values[ValueType.Resident].max.ToString();

            for (int i = 0; i < 3; i++)
            {
                castSlider[i].SetSlider(building.values[(ValueType)(i + 1)]);
            }

            ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
            for (int i = 0; i < residentialBuilding.existFacility.Length; i++)
            {
                SetOptionBuy(residentialBuilding, i);
            }
        }
        else
        {
            influenceText.text = (building.influencePower + building.additionalInfluencePower).ToString() + "(+" + building.additionalInfluencePower.ToString() + ")";
            valueTypeText.text = valueTypeString[(int)building.type - 1];
            utilitySlider.SetSlider(building.values[ValueType.utility]);
            userSlider.SetSlider(building.values[ValueType.user]);
        }

        List<Event> curEvent = building.GetEventProblem();

        for (int i = 0;i < eventUIInfos.Length; i++)
        {
            eventUIInfos[i].SetEventUIInfo(curEvent[i]);
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
            optionButtons[index].interactable = false;
        }
        else
        {
            ColorBlock block = optionButtons[index].colors;
            block.disabledColor = block.normalColor;
            optionButtons[index].colors = block;
            optionToggles[index].colors = block;
        }
    }

    private string GetSignString(int data, string zeroSign)
    {

        if (data > 0)
            return "+ " + data.ToString();
        else if (data < 0)
            return "- " + Mathf.Abs(data).ToString();
        else
            return zeroSign + " " + data.ToString();
    }
}
