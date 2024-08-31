using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PayFailUI : CreditUI
{
    public TextMeshProUGUI payDayText;
    public bool isFail;

    protected override void SetValue()
    {
        base.SetValue();

        int score = RoutineManager.instance.creditRating;
        int prevScore = isFail ? score + 25 : score;
        scoreBar.SetScore(prevScore, score);

        int debt = RoutineManager.instance.debt;
        totalMoneyText.text = GetCommaText(debt);

        payDayText.text = RoutineManager.instance.GetPayDay().ToString("yy.MM.dd");
    }
}
