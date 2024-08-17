using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReceiptUI : DebtUI
{
    protected override void SetValue()
    {
        base.SetValue();
        dayText.text = curDay.ToString("yyyy³â MM¿ù ddÀÏ");

        AkSoundEngine.PostEvent("Play_UI_papersound_001", gameObject);
    }

    protected void OnDisable()
    {
        int week = RoutineManager.instance.GetWeekOfYear() + 1;
        CityLevelManager.instance.UpdateCityType(week);
    }
}
