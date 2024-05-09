using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UtilityBuildingIntroUI : BuildingIntroUI
{
    [Header("Special")]
    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI valueTypeText;
    public ValueSlider utilitySlider;
    public ValueSlider userSlider;

    string[] valueTypeString = { "상품 가격", "입장료", "취업률" };

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        influenceText.text = (building.influencePower + building.additionalInfluencePower).ToString() + "(+" + building.additionalInfluencePower.ToString() + ")";
        valueTypeText.text = valueTypeString[(int)building.type - 1];
        SetSlider(utilitySlider, building.values[ValueType.utility]);
        SetSlider(userSlider, building.values[ValueType.user]);
    }
}
