using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType { Ground = -1, Road, Decoration }

public class Tile : MonoBehaviour
{
    public static int income;

    public string tileName;
    public TileType type;
    public bool isPurchased;
    public int cost { get; private set; }
    public int costPerDay { get; private set; }
    public int[] influenceValues;
    public int[] subInfluenceValues;
    public GameObject building;
    public MeshRenderer border;

    private void Awake()
    {
        influenceValues = new int[System.Enum.GetValues(typeof(BuildingType)).Length];
        subInfluenceValues = new int[System.Enum.GetValues(typeof(BuildingSubType)).Length];
    }

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

    public void SetInfluenceValue(BuildingType type, BuildingSubType subType, int value)
    {
        influenceValues[(int)type] += value;
        subInfluenceValues[(int)subType] += value >= 0 ? 1 : -1;

        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();

        buildCom.ApplyInfluence(value, type);
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
                buildCom.ApplyInfluence(influenceValues[i], (BuildingType)i);
            }
        }
        else
            buildCom.ApplyInfluence(influenceValues[(int)BuildingType.Residential], 0);
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