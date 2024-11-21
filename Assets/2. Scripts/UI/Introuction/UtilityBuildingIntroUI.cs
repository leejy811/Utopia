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

    public string[] valueTypeString = { "��ǰ ����", "�����", "Ȱ����" };

    public override void SetValue(Building building)
    {
        base.SetValue(building);

        if (building.happinessRate == 0)
        {
            influenceText.text = "0(" + (- building.influencePower - building.additionalInfluencePower) + ")";
        }
        else
        {
            influenceText.text = (building.influencePower + building.additionalInfluencePower).ToString() + "(" + GetSignString(building.additionalInfluencePower, "+") + ")";
        }
        valueTypeText.text = valueTypeString[(int)building.type - 1];
        SetParamText(building, (int)building.values[ValueType.user].CheckBoundary() + 1, (int)building.values[ValueType.utility].CheckBoundary() + 1);
    }

    public void SetParamText(Building building, int userBoundary, int utilityBoundary)
    {
        string[] boundString = { "����", "�����", "����" };
        string res = boundString[userBoundary] + " �̿��� ���� " + boundString[utilityBoundary] + " " + valueTypeString[(int)building.type - 1] + "�� ����";
        int colorIdx = 0;

        if (building.UpdateHappiness(true) != 0)
        {
            paramHappinessText.gameObject.SetActive(true);
            if (building.UpdateHappiness(true) > 0)
            {
                paramHappinessText.text = res + " <color=#00FF00>�ູ�� ���� ����</color>";
                colorIdx++;
            }
            else
                paramHappinessText.text = res + " <color=#FF0000>�ູ�� ���� ����</color>";
        }

        if (building.CalculateBonus(true) != 0)
        {
            paramBonusText.gameObject.SetActive(true);
            if (building.CalculateBonus(true) > 0)
            {
                paramBonusText.text = res + " <color=#00FF00>�߰� �ҵ� �߻� ����</color>";
                colorIdx++;
            }
            else
                paramBonusText.text = res + " <color=#FF0000>�߰� ���� �߻� ����</color>";
        }

        if(building.UpdateHappiness(true) == 0 && building.CalculateBonus(true) == 0)
        {
            paramHappinessText.gameObject.SetActive(true);
            paramHappinessText.text = "����";
        }

        SetSlider(utilitySlider, building.values[ValueType.utility], colorIdx);
        SetSlider(userSlider, building.values[ValueType.user], colorIdx);
    }

    protected override void SetInitState()
    {
        base.SetInitState();

        paramHappinessText.gameObject.SetActive(false);
        paramBonusText.gameObject.SetActive(false);
    }
}
