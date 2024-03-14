using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BuyState { None, BuyBuilding, SellBuilding, BuyTile, BuildTile }

public class ShopManager : MonoBehaviour
{
    static public ShopManager instance;

    public BuyState buyState;
    public int money;

    public GameObject[] buildingPrefabs;

    private GameObject curPickObject;
    private int curPickIndex;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void GetMoney(int amount)
    {
        money += amount;
    }

    private bool PayMoney(int amount)
    {
        if (money - amount < 0)
        {
            return false;
        }

        money -= amount;
        return true;
    }

    public void ChangeState(BuyState state, int index = 0)
    {
        if (state == buyState) return;

        curPickIndex = index;

        if (buyState == BuyState.BuyBuilding)
            Destroy(curPickObject);
        else if(state == BuyState.BuyBuilding)
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);

        buyState = state;
    }

    public void BuyBuilding(Transform spawnTrans)
    {
        int cost = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost;
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyBuilding) return;
        if (!PayMoney(cost)) return;
        if (!tile.CheckBuilding()) return;

        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);
    }

    public void SellBuilding(Transform spawnTrans)
    {
        int cost = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost;
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.SellBuilding) return;

        Destroy(tile.building);
        GetMoney(cost);
    }

    public void BuyTile(Transform tileTrans)
    {
        int cost = Grid.instance.tileCost;
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyTile) return;
        if (!PayMoney(cost)) return;
        if (!tile.CheckPurchased()) return;

        tile.SetTilePurchased(true);
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
            SetObjectColor(curPickObject, Color.white);
        else
            SetObjectColor(curPickObject, Color.red);
    }

    private void SetObjectColor(GameObject building, Color color)
    {
        Material mat = building.GetComponentInChildren<MeshRenderer>().material;
        mat.color = color;
    }
}
