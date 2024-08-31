using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditMailUI : MailUI
{
    public TextMeshProUGUI titleText;

    private string[] grade = { "D", "C", "B", "A", "S" };

    protected override void SetValue()
    {
        ResultType result = RoutineManager.instance.weekResult;
        int score = RoutineManager.instance.creditRating;
        
        if (result == ResultType.PayFail)
        {
            titleText.text = "신용 점수가 하락하였습니다.";
            dataText.text = "신용 점수가 " + (score + 25).ToString() + "에서 " + score.ToString() + "로 하락하였습니다.\n"
                                + "등급이 " + grade[Mathf.Min(score, 99) / 20] + "로 하락하였습니다.";
        }
        else
        {
            titleText.text = "신용 점수가 상승하였습니다.";
            dataText.text = "신용 점수가 " + (score - 10).ToString() + "에서 " + score.ToString() + "로 상승하였습니다.\n";

            if ((score - 10) / 20 != score / 20)
                dataText.text += "등급이 " + grade[Mathf.Min(score, 99) / 20] + "로 상승하였습니다.";
        }
    }
}
