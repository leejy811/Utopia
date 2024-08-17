using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCreditUI : ReceiptUI
{
    public CreditScoreBar scoreBar;

    protected override void SetValue()
    {
        base.SetValue();

        int score = RoutineManager.instance.creditRating;
        scoreBar.SetScore(score, Mathf.Min(score + 10, 100));
    }
}
