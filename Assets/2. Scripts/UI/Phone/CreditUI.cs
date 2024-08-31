using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditUI : MonoBehaviour
{
    public CreditScoreBar scoreBar;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI totalMoneyText;

    protected string[] grade = { "D", "C", "B", "A", "S" };

    protected void OnEnable()
    {
        SetValue();
    }

    protected virtual void SetValue()
    {
        totalMoneyText.text = GetCommaText(ShopManager.instance.Money);
        gradeText.text = grade[Mathf.Min(RoutineManager.instance.creditRating, 99) / 20] + " 등급";
    }

    protected string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString() + "원";
        else
            return string.Format("{0:#,###}", data) + "원";
    }
}
