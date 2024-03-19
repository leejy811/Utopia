using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BuyState { None, BuyBuilding, SellBuilding, BuyTile, BuildTile, BuyOption }

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

    public void GetMoney(int amount)
    {
        money += amount;
    }

    public bool PayMoney(int amount)
    {
        if (money - amount < 0)
        {
            return false;
        }

        money -= amount;
        return true;
    }

    public void ChangeState(BuyState state, int index = 0, GameObject pickObject = null)
    {
        if (state == buyState) return;

        curPickIndex = index;

        if (buyState == BuyState.BuyBuilding)
            Destroy(curPickObject);
        else if(state == BuyState.BuyBuilding)
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);

        if (buyState == BuyState.BuyOption)
            SetBuyOption(false, pickObject);
        else if (state == BuyState.BuyOption)
            SetBuyOption(true, pickObject);

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

    public void SellBuilding()
    {
        if (curPickObject == null) return;

        int cost = curPickObject.GetComponentInParent<Building>().cost;

        if (buyState != BuyState.SellBuilding) return;

        Destroy(curPickObject);
        GetMoney(cost);
    }

    public void BuyTile(Transform tileTrans)
    {
        int cost = Grid.instance.tileCost;
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyTile) return;
        if (!tile.CheckPurchased()) return;
        if (!PayMoney(cost)) return;

        tile.SetTilePurchased(true);
    }

    public void BuyOption(OptionType type)
    {
        ResidentialBuilding building = curPickObject.GetComponent<ResidentialBuilding>();

        if (buyState != BuyState.BuyOption) return;
        if (building.CheckFacility(type)) return;
        if (!PayMoney(0)) return;

        building.BuyFacility(type);
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
            SetObjectColor(curPickObject, Color.white);
        else
            SetObjectColor(curPickObject, Color.red);
    }

    public void SetObjectColor(GameObject building, Color color)
    {
        MeshRenderer[] renderers = building.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void SetSellBuilding(GameObject building)
    {
        if (curPickObject != null)
        {
            MeshRenderer[] renderers = curPickObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material.color = Color.white;
            }
        }

        if (building != null)
        {
            MeshRenderer[] renderers = building.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material.color = Color.red;
            }
        }

        curPickObject = building;
    }

    private void SetBuyOption(bool active, GameObject pickObject)
    {
        SetSellBuilding(null);
        curPickObject = pickObject;
        UIManager.instance.SetOptionPopUp(active);
    }
}
