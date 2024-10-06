using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class PayFailUI : CreditUI
{
    public TextMeshProUGUI payDayText;
    public bool isFail;

    protected override void SetValue()
    {
        base.SetValue();

        int score = data.score;
        int prevScore = isFail ? score + 25 : score;
        scoreBar.SetScore(prevScore, score);

        int debt = data.debt;
        totalMoneyText.text = GetCommaText(debt);

        payDayText.text = data.day.Load().ToString("yy.MM.dd");
    }
}
