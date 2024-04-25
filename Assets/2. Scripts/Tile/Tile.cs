using System;
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
        SetTileColor(isPurchased);

        this.isPurchased = isPurchased;
    }

    public void SetInfluenceValue(BuildingType type, int value)
    {
        influenceValues[(int)type] += value;

        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();
        buildCom.ApplyInfluence(value, type);
    }

    public void SetSubInfluenceValue(BuildingSubType subType, bool enable)
    {
        subInfluenceValues[(int)subType] += enable ? 1 : -1;

        if (building == null) return;

        Building buildCom = building.GetComponent<Building>();
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

    public void SetTileColor(bool isPurchased)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer == border) continue;

            renderer.material.color = Grid.instance.tilePurchaseColors[Convert.ToInt32(isPurchased)];
        }
    }

    public void Coloring(bool isOn)
    {
        Building building = this.building.GetComponent<Building>();
        Color color = isOn ? Grid.instance.tileColors[(int)building.type] : Grid.instance.tilePurchaseColors[1];

        SetTileColor(color);
        building.ChangeViewState((ViewStateType)(Convert.ToInt32(!isOn) * 2));
    }
}