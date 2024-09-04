using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public TextMeshProUGUI[] castTexts;
    public TextMeshProUGUI bonusText;

    string[] optionString = { "수도", "하수", "전력", "방음" };

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        residentText.text = building.values[ValueType.Resident].cur.ToString() + "/" + building.values[ValueType.Resident].max.ToString();

        for (int i = 0; i < 3; i++)
        {
            ValueType type = (ValueType)(i + 1);
            SetSlider(castSlider[i], building.values[type], (int)building.values[type].CheckBoundary() + 1);
            castTexts[i].text = GetCastString(building, (int)building.values[type].CheckBoundary() + 1, (int)type);
        }

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
        for (int i = 0; i < residentialBuilding.existFacility.Length; i++)
        {
            SetOptionBuy(residentialBuilding, i);
        }

        bonusText.text = "만족도로 인해 ";
        if (building.CalculateBonus(true) != 0)
        {
            bonusText.gameObject.SetActive(true);
            if (building.CalculateBonus(true) > 0)
                bonusText.text += " <color=#00FF00>추가 소득 발생 예정</color>";
            else
                bonusText.text += " <color=#FF0000>추가 지출 발생 예정</color>";
        }
        else
        {
            bonusText.gameObject.SetActive(false);
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
            optionButtons[index].interactable = true;
        }
    }

    protected string GetCastString(Building building, int boundaryIdx, int type)
    {
        string res = "";
        string castEffectString = "";

        if (building.UpdateHappiness(true, type) != 0)
            castEffectString = building.UpdateHappiness(true, type) > 0 ? "<color=#00FF00>행복도 증가" : "<color=#FF0000>행복도 감소";
        else if (building.CalculateBonus(true) != 0)
            castEffectString = "<color=#FF0000>추가 지출 발생";

        res += boundaryString[boundaryIdx] + "한 " + typeString[type] + " 만족도로 인해 " + castEffectString + " 예정</color>";
        return res;
    }
}
