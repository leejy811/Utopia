using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UtilityBuildingIntroUI_Totopia : UtilityBuildingIntroUI
{
    [Header("Totopia")]
    public GameObject minigamePanel;
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI betTimesText;
    public Button gameButton;

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        if (building.type == BuildingType.Culture)
        {
            minigamePanel.SetActive(true);

            EnterBuilding enterBuilding = building as EnterBuilding;
            betChipText.text = ((int)enterBuilding.values[ValueType.betChip].cur).ToString();
            betTimesText.text = enterBuilding.betTimes.ToString();

            if (enterBuilding.betTimes == 0)
                gameButton.interactable = false;
            else
                gameButton.interactable = true;
        }
        else
            minigamePanel.SetActive(false);
    }
}
