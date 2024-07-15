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
    public TextMeshProUGUI[] castTexts;

    string[] optionString = { "����", "�ϼ�", "����", "����" };

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
            castEffectString = building.UpdateHappiness(true, type) > 0 ? "<color=#00FF00>�ູ�� ����" : "<color=#FF0000>�ູ�� ����";
        else if (building.CalculateBonus(true) != 0)
            castEffectString = "<color=#FF0000>�߰� ���� �߻�";

        res += boundaryString[boundaryIdx] + "�� " + typeString[type] + " �������� ���� " + castEffectString + " ����</color>";
        return res;
    }
}
