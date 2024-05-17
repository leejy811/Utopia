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
    public TextMeshProUGUI paramHappinessText;
    public TextMeshProUGUI paramBonusText;

    string[] valueTypeString = { "��ǰ ����", "�����", "�����" };

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        influenceText.text = (building.influencePower + building.additionalInfluencePower).ToString() + "(" + GetSignString(building.additionalInfluencePower, "+") + ")";
        valueTypeText.text = valueTypeString[(int)building.type - 1];
        SetSlider(utilitySlider, building.values[ValueType.utility]);
        SetSlider(userSlider, building.values[ValueType.user]);
        SetParamText(building, (int)building.values[ValueType.user].CheckBoundary() + 1, (int)building.values[ValueType.utility].CheckBoundary() + 1);
    }

    public void SetParamText(Building building, int userBoundary, int utilityBoundary)
    {
        string[] boundString = { "����", "�����", "����" };
        string res = boundString[userBoundary] + " �̿��� ���� " + boundString[utilityBoundary] + " " + valueTypeString[(int)building.type - 1] + "�� ����";

        if (building.UpdateHappiness(true) != 0)
        {
            paramHappinessText.gameObject.SetActive(true);
            paramHappinessText.text = res + (building.UpdateHappiness(true) > 0 ? " <color=#00FF00>�ູ�� ���� ����" : " <color=#FF0000>�ູ�� ���� ����") + "</color>";
        }

        if (building.CalculateBonus(true) != 0)
        {
            paramBonusText.gameObject.SetActive(true);
            paramBonusText.text = res + (building.CalculateBonus(true) > 0 ? " <color=#00FF00>�߰� �ҵ� �߻� ����" : " <color=#FF0000>�߰� ���� �߻� ����") + "</color>";
            return;
        }

        if(building.UpdateHappiness(true) == 0 && building.CalculateBonus(true) == 0)
        {
            paramHappinessText.gameObject.SetActive(true);
            paramHappinessText.text = "����";
        }
    }

    protected override void SetInitState()
    {
        base.SetInitState();

        paramHappinessText.gameObject.SetActive(false);
        paramBonusText.gameObject.SetActive(false);
    }
}
