using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfoUI : InfoUI
{
    [Header("Tile")]
    public TextMeshProUGUI tileCostText;
    public TextMeshProUGUI tileCostPerDayText;

    string[] nameString = { "µµ·Î", "¹°", "½£" };

    public override void SetValue(int index)
    {
        gameObject.SetActive(!gameObject.activeSelf);
        nameText.text = nameString[index];
        tileCostText.text = Grid.instance.tileCost[index + 1].ToString();
        tileCostPerDayText.text = Grid.instance.tileCostPerDay[index].ToString();
    }
}
