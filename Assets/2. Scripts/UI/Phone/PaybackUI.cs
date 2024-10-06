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

        int score = data.score;
        scoreBar.SetScore(score, score);

        int curMoney = data.money;
        int debt = data.debt;
        int paybackMoney = (int)(debt * 0.2f);
        debtText.text = "-" + GetCommaText(debt);
        paybackText.text = "+" + GetCommaText(paybackMoney);
        curMoneyText.text = GetCommaText(curMoney + debt - paybackMoney);

        foreach (var dayText in curDayTexts)
        {
            dayText.text = data.day.Load().ToString("yy.MM.dd");
        }
    }
}
