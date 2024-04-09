using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfoUI : MonoBehaviour
{
    [Header("Tile")]
    public TextMeshProUGUI tileNameText;
    public TextMeshProUGUI tileTypeText;
    public TextMeshProUGUI tileCostText;
    public TextMeshProUGUI tileCostPerDayText;

    string[] typeString = { "도로", "조경" };

    public void SetValue(Tile tile)
    {
        tileNameText.text = tile.tileName;
        tileTypeText.text = typeString[(int)tile.type];
        tileCostText.text = tile.cost.ToString();
        tileCostPerDayText.text = tile.costPerDay.ToString();
    }

    public void OnUI(Tile tile, Vector3 pos)
    {
        transform.localPosition = pos;
        SetValue(tile);
    }
}
