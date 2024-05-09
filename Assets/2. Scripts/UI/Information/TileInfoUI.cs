using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfoUI : InfoUI
{
    [Header("Tile")]
    public TextMeshProUGUI tileCostText;
    public TextMeshProUGUI tileCostPerDayText;

    string[] typeString = { "도로", "조경" };

    public override void SetValue(int index)
    {
        gameObject.SetActive(!gameObject.activeSelf);

        //ToDo 타일 건설 완료 후 tile Gameobject 연결

        //nameText.text = tile.tileName;
        //typeText.text = typeString[(int)tile.type];
        //tileCostText.text = tile.cost.ToString();
        //tileCostPerDayText.text = tile.costPerDay.ToString();
    }
}
