using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResidentialBuildingIntroUI : BuildingIntroUI
{
    [Header("Special")]
    public TextMeshProUGUI residentText; 
    public ValueSlider[] castSlider;
    public Button[] optionButtons;
    public Toggle[] optionToggles;
    public TextMeshProUGUI[] optionTexts;
    public TextMeshProUGUI[] castTexts;

    string[] optionString = { "수도", "하수", "전력", "방음" };

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        residentText.text = building.values[ValueType.Resident].cur.ToString() + "/" + building.values[ValueType.Resident].max.ToString();

        for (int i = 0; i < 3; i++)
        {
            ValueType type = (ValueType)(i + 1);
            SetSlider(castSlider[i], building.values[type]);
            castTexts[i].text = GetCastString(building, (int)building.values[type].CheckBoundary() + 1, (int)type);
        }

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
        int cnt = 0;
        for (int i = 0; i < residentialBuilding.existFacility.Length; i++)
        {
            SetOptionBuy(residentialBuilding, i);

            if (residentialBuilding.CheckFacility((OptionType)i))
            {
                optionTexts[i].gameObject.SetActive(true);
                optionTexts[i].text = GetOptionString(i);
                cnt++;
            }
        }

        if(cnt == 0)
        {
            optionTexts[0].gameObject.SetActive(true);
            optionTexts[0].text = "없음";
        }
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

    private string GetOptionString(int type)
    {
        string res = optionString[type] + "옵션 설치로 인해 특정 사회현상 발생 차단";
        return res;
    }

    protected string GetCastString(Building building, int boundaryIdx, int type)
    {
        string res = "";
        string castEffectString = "";

        if (building.UpdateHappiness(true, type) != 0)
            castEffectString = building.UpdateHappiness(true, type) > 0 ? "행복도 증가" : "행복도 감소";
        else if (building.CalculateBonus(true) != 0)
            castEffectString = "추가 지출 발생";

        res += boundaryString[boundaryIdx] + "한 " + typeString[type] + " 만족도로 인해 " + castEffectString + " 예정";
        return res;
    }

    protected override void SetInitState()
    {
        base.SetInitState();

        foreach (TextMeshProUGUI text in optionTexts)
        {
            text.gameObject.SetActive(false);
        }
    }
}
