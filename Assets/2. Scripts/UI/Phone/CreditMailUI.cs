using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditMailUI : MailUI
{
    public TextMeshProUGUI titleText;

    public override void SetValue(MailData data)
    {
        CreditPanelData creditData = data.creditPanelData;
        ResultType result = creditData.result;
        int score = creditData.score;
        
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

        dataText.text += data.mailDay.Load().ToString("yyyy.MM.dd");
    }
}
