using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtUI : UIElement
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    protected virtual void SetValue()
    {
        int debtMoney = RoutineManager.instance.debt;
        DateTime curDay = RoutineManager.instance.day;
        int dayOfWeek = curDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)curDay.DayOfWeek;

        if (state == EventState.Receipt)
            gameObject.GetComponentInChildren<Animator>().SetInteger("DayOfWeek", dayOfWeek);
        else
            curDay = curDay.AddDays(8 - dayOfWeek);

        moneyText.text = "채무금 : " + debtMoney.ToString("C");
        dayText.text = "지급기일 : " + curDay.ToString("yyyy년 MM월 dd일") + " 00시";
    }

    protected void OnEnable()
    {
        SetValue();
    }
}
