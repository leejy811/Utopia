using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealCreditUI : CreditUI
{
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI curMoneyText;
    public TextMeshProUGUI[] curDayTexts;

    protected override void SetValue()
    {
        base.SetValue();

        int score = RoutineManager.instance.creditRating;
        scoreBar.SetScore(score - 10, score);

        int curMoney = ShopManager.instance.Money;
        int week = RoutineManager.instance.GetWeekOfYear();
        int debt = RoutineManager.instance.debtsOfWeek[week];
        debtText.text = "-" + GetCommaText(debt);
        curMoneyText.text = GetCommaText(curMoney + debt);

        foreach (var dayText in curDayTexts)
        {
            dayText.text = RoutineManager.instance.day.ToString("yy.MM.dd");
        }
    }
}
