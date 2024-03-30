using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType { Ground, Road, Decoration }

public class Tile : MonoBehaviour
{
    public TileType type;
    public bool isPurchased;
    public int costPerDay { get; private set; }
    public int[] influenceValues = new int[System.Enum.GetValues(typeof(BuildingType)).Length];
    public int[] subInfluenceValues = new int[System.Enum.GetValues(typeof(BuildingSubType)).Length];
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
        if (isPurchased)
            SetTileColor(Color.green);
        else
            SetTileColor(Color.red);

        this.isPurchased = isPurchased;
    }

    public void SetInfluenceValue(BuildingType type, BuildingSubType subType, int value, bool isAdd)
    {
        influenceValues[(int)type] += isAdd ? value : -value;
        subInfluenceValues[(int)subType] += isAdd ? 1 : -1;

        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();

        buildCom.ApplyInfluence(value, isAdd, type);
        buildCom.SolveEventToInfluence(subType);
    }

    public void ApplyInfluenceToBuilding()
    {
        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();

        if (buildCom.type == BuildingType.Residential)
        {
            for (int i = 1;i < influenceValues.Length;i++)
            {
                buildCom.ApplyInfluence(influenceValues[i], true, (BuildingType)i);
            }
        }
        else
            buildCom.ApplyInfluence(influenceValues[(int)BuildingType.Residential], true, 0);
    }

    public void SetTileColor(Color color)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer == border) continue;

            renderer.material.color = color;
        }
    }
}