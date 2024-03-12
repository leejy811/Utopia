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
    public int money { get; private set; } = 20;

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

    private void GetCoin(int amount)
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

        if (buyState != BuyState.BuyBuilding) return;
        if (!PayMoney(cost)) return;

        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);
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
}
