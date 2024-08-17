using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtUI : UIElement
{
    [Header("UIComponent")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    protected DateTime curDay;
    protected int debtMoney;
    protected int dayOfWeek;

    protected virtual void SetValue()
    {
        CalculateValue();
        moneyText.text = debtMoney.ToString("C");
        dayText.text = curDay.ToString("yyyy년 MM월 dd일") + " 00시";
    }

    protected virtual void CalculateValue()
    {
        debtMoney = RoutineManager.instance.debt;
        curDay = RoutineManager.instance.day;
    }

    protected void OnEnable()
    {
        SetValue();
    }
}
