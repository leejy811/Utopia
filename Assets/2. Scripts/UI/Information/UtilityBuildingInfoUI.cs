using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UtilityBuildingInfoUI : BuildingInfoUI
{
    [Header("Special")]
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI costPerDayText;

    public override void SetValue(int index)
    {
        base.SetValue(index);

        influenceText.text = building.influencePower.ToString();

        if (building.type == BuildingType.Service)
            costPerDayText.text = (building as ServiceBuilding).costPerDay.ToString();
    }
}
