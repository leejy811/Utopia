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
        {
            gameObject.GetComponentInChildren<Animator>().SetInteger("DayOfWeek", dayOfWeek);
            AkSoundEngine.PostEvent("Play_stamp", gameObject);
        }
         else
            curDay = curDay.AddDays(8 - dayOfWeek);
        AkSoundEngine.PostEvent("Play_UI_papersound_001", gameObject);
        moneyText.text = debtMoney.ToString("C");
        dayText.text = curDay.ToString("yyyy년 MM월 dd일") + " 00시";
    }

    protected void OnEnable()
    {
        SetValue();
    }
}
