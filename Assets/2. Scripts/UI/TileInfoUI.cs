using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfoUI : InfoUI
{
    [Header("Tile")]
    public TextMeshProUGUI tileCostText;
    public TextMeshProUGUI tileCostPerDayText;

    string[] typeString = { "����", "����" };

    public override void SetValue(int index)
    {
        gameObject.SetActive(!gameObject.activeSelf);

        //ToDo Ÿ�� �Ǽ� �Ϸ� �� tile Gameobject ����

        //nameText.text = tile.tileName;
        //typeText.text = typeString[(int)tile.type];
        //tileCostText.text = tile.cost.ToString();
        //tileCostPerDayText.text = tile.costPerDay.ToString();
    }
}
