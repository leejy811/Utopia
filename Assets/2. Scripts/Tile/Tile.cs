using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Ground, Road, Decoration }

[Serializable]
public class InfluenceValue
{
    public BuildingType type;
    public int value;

    public InfluenceValue(BuildingType type, int value)
    {
        this.type = type;
        this.value = value;
    }

    public bool CheckEqual(InfluenceValue inValue)
    {
        return type == inValue.type && value == inValue.value;
    }
}

public class Tile : MonoBehaviour
{
    public TileType type;
    public bool isPurchased { get; private set; }
    public int costPerDay { get; private set; }
    public GameObject building;
    public List<InfluenceValue> influenceValues;

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

    public void RemoveInfluence(InfluenceValue value)
    {
        for(int i = 0;i < influenceValues.Count; i++)
        {
            if (influenceValues[i].Equals(value))
                influenceValues.RemoveAt(i);
        }
    }
}