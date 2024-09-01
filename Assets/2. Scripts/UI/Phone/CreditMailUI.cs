using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditMailUI : MailUI
{
    public TextMeshProUGUI titleText;

    public override void SetValue()
    {
        ResultType result = RoutineManager.instance.weekResult;
        int score = RoutineManager.instance.creditRating;
        
        if (result == ResultType.PayFail)
        {
            titleText.text = "신용 점수가 하락하였습니다.";
            dataText.text = "신용 점수가 " + (score + 25).ToString() + "에서 " + score.ToString() + "로 하락하였습니다.\n";
        }
        else
        {
            titleText.text = "신용 점수가 상승하였습니다.";
            dataText.text = "신용 점수가 " + (score - 10).ToString() + "에서 " + score.ToString() + "로 상승하였습니다.\n";
        }

        dataText.text += RoutineManager.instance.day.ToString("yyyy.MM.dd");
    }
}
