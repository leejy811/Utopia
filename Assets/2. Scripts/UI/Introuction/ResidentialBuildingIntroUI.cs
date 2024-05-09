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

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        residentText.text = building.values[ValueType.Resident].cur.ToString() + "/" + building.values[ValueType.Resident].max.ToString();

        for (int i = 0; i < 3; i++)
        {
            SetSlider(castSlider[i], building.values[(ValueType)(i + 1)]);
        }

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
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
        }
    }
}
