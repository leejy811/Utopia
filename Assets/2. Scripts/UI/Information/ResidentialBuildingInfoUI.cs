using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResidentialBuildingInfoUI : BuildingInfoUI
{
    [Header("Special")]
    public TextMeshProUGUI residentText;
    public TextMeshProUGUI commercialCsatText;
    public TextMeshProUGUI cultureCsatText;
    public TextMeshProUGUI serviceCsatText;

    public override void SetValue(int index)
    {
        base.SetValue(index);

        ResidentialBuilding residentialBuilding = building as ResidentialBuilding;

        residentText.text = residentialBuilding.residentCnt.max.ToString();
        commercialCsatText.text = residentialBuilding.commercialCSAT.min.ToString() + " ~ " + residentialBuilding.commercialCSAT.max.ToString();
        cultureCsatText.text = residentialBuilding.cultureCSAT.min.ToString() + " ~ " + residentialBuilding.cultureCSAT.max.ToString();
        serviceCsatText.text = residentialBuilding.serviceCSAT.min.ToString() + " ~ " + residentialBuilding.serviceCSAT.max.ToString();
    }
}
