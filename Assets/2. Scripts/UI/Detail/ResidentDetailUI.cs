using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResidentDetailUI : DetailUI
{
    public TextMeshProUGUI[] residentText;

    public override void SetValue()
    {
        int[] resident = BuildingSpawner.instance.GetBuildingsResident();

        for(int i = 0;i < resident.Length;i++)
        {
            residentText[i].text = resident[i].ToString() + "Έν";
        }
    }
}
