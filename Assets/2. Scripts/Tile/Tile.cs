using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Ground, Road, Decoration}
public class Tile : MonoBehaviour
{
    public TileType type;
    public bool isPurchased { get; private set; }
    [SerializeField] private int cost;
    [SerializeField] private int costPerDay;

    public bool CheckBuilding()
    {
        return type == TileType.Ground && isPurchased;
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
}