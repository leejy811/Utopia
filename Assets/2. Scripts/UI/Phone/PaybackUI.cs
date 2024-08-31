using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaybackUI : CreditUI
{
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI paybackText;
    public TextMeshProUGUI curMoneyText;
    public TextMeshProUGUI[] curDayTexts;

    protected override void SetValue()
    {
        base.SetValue();

        int score = RoutineManager.instance.creditRating;
        scoreBar.SetScore(score, score);

        int curMoney = ShopManager.instance.Money;
        int week = RoutineManager.instance.GetWeekOfYear();
        int debt = RoutineManager.instance.debtsOfWeek[week];
        int paybackMoney = (int)(debt * 0.2f);
        debtText.text = "-" + GetCommaText(debt);
        paybackText.text = "+" + GetCommaText(paybackMoney);
        curMoneyText.text = GetCommaText(curMoney + debt - paybackMoney);

        foreach (var dayText in curDayTexts)
        {
            dayText.text = RoutineManager.instance.day.ToString("yy.MM.dd");
        }
    }
}
