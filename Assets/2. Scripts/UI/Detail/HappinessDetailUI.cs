using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HappinessDetailUI : DetailUI
{
    public TextMeshProUGUI[] happinessText;

    public override void SetValue()
    {
        int[] happiness = BuildingSpawner.instance.GetBuildingsHappiness();

        for(int i = 0;i < happinessText.Length;i++)
        {
            happinessText[i].text = happiness[i].ToString() + "%";
        }
    }
}
