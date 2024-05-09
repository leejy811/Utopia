using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventUIInfo : BuildingEventUIInfo
{
    public TextMeshProUGUI effectText;

    public override void SetEventUIInfo(Event curEvent)
    {
        base.SetEventUIInfo(curEvent);
        //effectText.text = curEvent.eventEffectComment;
    }
}
