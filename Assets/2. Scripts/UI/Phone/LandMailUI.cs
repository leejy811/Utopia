using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMailUI : MailUI
{
    public override void SetValue()
    {
        dataText.text = RoutineManager.instance.day.ToString("yyyy.MM.dd");
    }
}
