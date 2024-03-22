using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType { Ground, Road, Decoration }

public class Tile : MonoBehaviour
{
    public TileType type;
    public bool isPurchased { get; private set; }
    public int costPerDay { get; private set; }
    public int[] influenceValues = new int[3];
    public int[] subInfluenceValues = new int[7];
    public GameObject building;
    public MeshRenderer border;

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
            if (renderer == border) continue;
            if (isPurchased)
                renderer.material.color = Color.green;
            else
                renderer.material.color = Color.red;
        }

        this.isPurchased = isPurchased;
    }

    public void SetInfluenceValue(BuildingType type, BuildingSubType subType, int value, bool isAdd)
    {
        influenceValues[(int)type] += isAdd ? value : -value;
        subInfluenceValues[(int)type] += isAdd ? 1 : -1;

        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();

        if (buildCom.type == BuildingType.Residential)
        {
            (buildCom as ResidentialBuilding).ApplyInfluence(type, value, isAdd);
        }
    }

    public void ApplyInfluenceToBuilding()
    {
        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();

        if (buildCom.type == BuildingType.Residential)
        {
            for (int i = 0;i < influenceValues.Length;i++)
            {
                (buildCom as ResidentialBuilding).ApplyInfluence((BuildingType)i, influenceValues[i], true);
            }
        }
    }
}