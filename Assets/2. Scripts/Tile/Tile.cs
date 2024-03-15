using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Ground, Road, Decoration }

public class Tile : MonoBehaviour
{
    public TileType type;
    public bool isPurchased { get; private set; }
    public int costPerDay { get; private set; }
    public int[] influenceValues = new int[3];
    public GameObject building;

    public bool CheckBuilding()
    {
        return type == TileType.Ground && isPurchased && building == null;
    }

    public bool CheckPurchased()
    {
        return !isPurchased;
    }

    public void SetTilePurchased(bool isPurchased)
    {
        Material mat = gameObject.GetComponent<MeshRenderer>().material;

        this.isPurchased = isPurchased;

        if (isPurchased)
            mat.color = Color.green;
        else
            mat.color = Color.red;
    }

    public void SetInfluenceValue(BuildingType type, int value, bool isAdd)
    {
        influenceValues[(int)type] += isAdd ? value : -value;
    }
}