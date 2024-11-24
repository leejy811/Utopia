using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UtilityBuildingInfoUI : BuildingInfoUI
{
    [Header("Special")]
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI userCntText;
    public TextMeshProUGUI randomParameterText;

    private string[] parameterString = { "낮음", "보통", "높음" };

    public override void SetValue(int index)
    {
        base.SetValue(index);
        UtilityBuilding utilityBuilding = building as UtilityBuilding;

        influenceText.text = building.influencePower.ToString();
        userCntText.text = utilityBuilding.userCnt.min.ToString() + "~" + utilityBuilding.userCnt.max.ToString();

        int sign = BuildingSpawner.instance.randomParameter[index];
        int parameter = (int)utilityBuilding.GetRandomParameter(sign);
        randomParameterText.text = parameter.ToString() + "(" + parameterString[sign + 1] + ")";
    }
}
