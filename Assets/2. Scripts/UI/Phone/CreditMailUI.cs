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
            titleText.text = "�ſ� ������ �϶��Ͽ����ϴ�.";
            dataText.text = "�ſ� ������ " + (score + 25).ToString() + "���� " + score.ToString() + "�� �϶��Ͽ����ϴ�.\n";
        }
        else
        {
            titleText.text = "�ſ� ������ ����Ͽ����ϴ�.";
            dataText.text = "�ſ� ������ " + (score - 10).ToString() + "���� " + score.ToString() + "�� ����Ͽ����ϴ�.\n";
        }

        dataText.text += RoutineManager.instance.day.ToString("yyyy.MM.dd");
    }
}
