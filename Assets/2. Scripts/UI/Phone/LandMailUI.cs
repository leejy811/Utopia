using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMailUI : MailUI
{
    public override void SetValue(MailData data)
    {
        dataText.text = data.mailDay.Load().ToString("yyyy.MM.dd");
    }
}
