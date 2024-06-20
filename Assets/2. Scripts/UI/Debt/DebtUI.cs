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

        moneyText.text = "��ȯ �䱸�� : " + debtMoney.ToString("C");
        dayText.text = "������ : " + curDay.ToString("yyyy�� MM�� dd��");
    }

    protected void OnEnable()
    {
        SetValue();
    }
}
