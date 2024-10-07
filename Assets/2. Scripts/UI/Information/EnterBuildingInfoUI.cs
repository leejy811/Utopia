using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterBuildingInfoUI : UtilityBuildingInfoUI
{
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI betTimesText;

    public override void SetValue(int index)
    {
        base.SetValue(index);
        EnterBuilding enterBuilding = building as EnterBuilding;

        randomParameterText.text = enterBuilding.utilityValue.min.ToString() + " ~ " + enterBuilding.utilityValue.max.ToString();
        
        int sign = BuildingSpawner.instance.randomParameter[index];
        int parameter = (int)enterBuilding.GetRandomParameter(sign);
        betChipText.text = parameter.ToString();
        betTimesText.text = enterBuilding.betTimes.ToString();
    }
}
