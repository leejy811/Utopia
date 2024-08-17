using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType { Ground = -1, Road, Water, Forest }

public class Tile : MonoBehaviour
{
    public static int income;

    public string tileName;
    public TileType type;
    public Vector2 tilePos;
    public bool isPurchased;

    public int[] influenceValues;
    public int[] subInfluenceValues;
    public GameObject building;

    public GameObject[] tileModels;
    public GameObject targetTile;

    private void Awake()
    {
        influenceValues = new int[System.Enum.GetValues(typeof(BuildingType)).Length];
        subInfluenceValues = new int[System.Enum.GetValues(typeof(BuildingSubType)).Length];
    }

    public bool CheckBuilding()
    {
        return type == TileType.Ground && isPurchased && building == null;
    }

    public bool CheckBuildingWithErrorMsg()
    {
        if (type == TileType.Ground && isPurchased && building == null)
        {
            return true;
        }
        else
        {
            string[] str = { "해당 위치에는 건설할 수 없습니다." };
            UIManager.instance.SetErrorPopUp(str, transform.position);
            return false;
        }
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
                buildCom.ApplyInfluence(influenceValues[i], (BuildingType)i, true);
            }
        }
        else
            buildCom.ApplyInfluence(influenceValues[(int)BuildingType.Residential], 0, true);
    }

    public void SetTileColor(Color color)
    {
        if (!targetTile.activeSelf) return;

        MeshRenderer renderer = targetTile.GetComponent<MeshRenderer>();
        renderer.material.color = color;
    }

    public void SetTileColor(bool isPurchased)
    {
        if (!targetTile.activeSelf) return;

        MeshRenderer renderer = targetTile.GetComponent<MeshRenderer>();
        renderer.material.color = Grid.instance.tilePurchaseColors[Convert.ToInt32(isPurchased)];
    }

    public void Coloring(bool isOn)
    {
        Building building = this.building.GetComponent<Building>();
        Color color = isOn ? Grid.instance.tileColors[(int)building.type] : Grid.instance.tilePurchaseColors[1];

        SetTileColor(color);
        building.ChangeViewState((ViewStateType)(Convert.ToInt32(!isOn) * 2));
    }

    public void ChangeTileModel(int index)
    {
        for(int i = 0;i < tileModels.Length; i++)
        {
            if (i == index + 1)
                tileModels[i].SetActive(true);
            else
                tileModels[i].SetActive(false);
        }

        type = (TileType)index;
        ApplyHappinessToBuilding(type != TileType.Ground);
        ReplaceTile();
    }

    public void ReplaceTile()
    {
        if (type == TileType.Road)
            gameObject.GetComponentInChildren<RoadPlacement>().PlaceRoad(transform);
        else if (type == TileType.Water)
        {
            gameObject.GetComponentInChildren<WaterPlacement>().PlaceWater(transform);
        }
    }

    public int CaculateIncome()
    {
        int res = 0;

        if (type != TileType.Ground)
            res -= Grid.instance.tileCostPerDay[(int)type];

        income += res;
        return res;
    }

    private void ApplyHappinessToBuilding(bool isAdd)
    {
        int influenceSize = 2;
        int sign = isAdd ? 1 : -1;
        Vector3 size = new Vector3(influenceSize * 2 - 1, 3, influenceSize * 2 - 1);

        Collider[] hits = Physics.OverlapBox(transform.position, size * 0.5f, Quaternion.identity, LayerMask.GetMask("Building"));

        foreach (Collider hit in hits)
        {
            Building building = hit.transform.gameObject.GetComponent<Building>();
            building.SetHappiness(2 * sign);
        }
    }
}