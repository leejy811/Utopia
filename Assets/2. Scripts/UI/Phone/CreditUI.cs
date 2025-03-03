using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditUI : MonoBehaviour
{
    public CreditScoreBar scoreBar;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI totalMoneyText;

    public CreditPanelData data;

    protected string[] grade = { "D", "C", "B", "A", "S" };

    protected void OnEnable()
    {
        SetValue();
    }

    protected virtual void SetValue()
    {
        totalMoneyText.text = GetCommaText(data.money);
        gradeText.text = grade[Mathf.Min(data.score, 99) / 20] + " ���";
    }

    protected string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString() + "��";
        else
            return string.Format("{0:#,###}", data) + "��";
    }
}
