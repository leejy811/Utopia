using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResidentialBuildingInfoUI : BuildingInfoUI
{
    [Header("Special")]
    public TextMeshProUGUI residentText;
    public Button[] optionButtons;
    public Toggle[] optionToggles;

    public override void SetValue(int index)
    {
        base.SetValue(index);

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;

        residentText.text = residentialBuilding.residentCnt.max.ToString(); // + "/" + residentialBuilding.residentCnt.max.ToString();

        //for (int i = 0; i < residentialBuilding.existFacility.Length; i++)
        //{
        //    SetOptionBuy(residentialBuilding, i);
        //}
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
