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
            titleText.text = "�ſ� ������ �϶��Ͽ����ϴ�.";
            dataText.text = "�ſ� ������ " + (score + 25).ToString() + "���� " + score.ToString() + "�� �϶��Ͽ����ϴ�.\n"
                                + "����� " + grade[Mathf.Min(score, 99) / 20] + "�� �϶��Ͽ����ϴ�.";
        }
        else
        {
            titleText.text = "�ſ� ������ ����Ͽ����ϴ�.";
            dataText.text = "�ſ� ������ " + (score - 10).ToString() + "���� " + score.ToString() + "�� ����Ͽ����ϴ�.\n";

            if ((score - 10) / 20 != score / 20)
                dataText.text += "����� " + grade[Mathf.Min(score, 99) / 20] + "�� ����Ͽ����ϴ�.";
        }
    }
}
