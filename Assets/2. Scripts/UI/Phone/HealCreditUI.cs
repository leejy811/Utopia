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

        int score = data.score;
        scoreBar.SetScore(score - 10, score);

        int curMoney = data.money;
        int debt = data.debt;
        debtText.text = "-" + GetCommaText(debt);
        curMoneyText.text = GetCommaText(curMoney + debt);

        foreach (var dayText in curDayTexts)
        {
            dayText.text = data.day.Load().ToString("yy.MM.dd");
        }
    }
}
