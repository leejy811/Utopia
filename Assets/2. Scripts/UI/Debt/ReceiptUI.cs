using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiptUI : DebtUI
{
    protected override void SetValue()
    {
        base.SetValue();

        gameObject.GetComponentInChildren<Animator>().SetInteger("DayOfWeek", dayOfWeek);
        AkSoundEngine.PostEvent("Play_stamp", gameObject);
        AkSoundEngine.PostEvent("Play_UI_papersound_001", gameObject);
    }

    private void OnDisable()
    {
        int week = RoutineManager.instance.GetWeekOfYear() + 1;
        CityLevelManager.instance.UpdateCityType(week);
    }
}
