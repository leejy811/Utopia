using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingEventUIInfo : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dayText;

    public virtual void SetEventUIInfo(Event curEvent)
    {
        iconImage.sprite = curEvent.eventIcon;
        nameText.text = curEvent.eventName.ToString();
        dayText.text = "(D-" + (curEvent.effectValue.Count - curEvent.curDay).ToString() + ")";
    }
}
