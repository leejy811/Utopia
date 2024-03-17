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
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (isPurchased)
                renderer.material.color = Color.green;
            else
                renderer.material.color = Color.red;
        }

        this.isPurchased = isPurchased;
    }

    public void SetInfluenceValue(BuildingType type, int value, bool isAdd)
    {
        influenceValues[(int)type] += isAdd ? value : -value;
    }
}