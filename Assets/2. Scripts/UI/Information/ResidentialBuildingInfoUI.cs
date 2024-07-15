using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResidentialBuildingInfoUI : BuildingInfoUI
{
    [Header("Special")]
    public TextMeshProUGUI residentText;

    public override void SetValue(int index)
    {
        base.SetValue(index);

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;

        residentText.text = residentialBuilding.residentCnt.max.ToString();
    }
}
