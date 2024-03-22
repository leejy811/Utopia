using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BuyState { None, BuyBuilding, SellBuilding, BuyTile, BuildTile, BuyOption, SolveEvent }

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
        if (state == buyState)
        {

            return;
        }

        curPickIndex = index;

        if (buyState == BuyState.BuyBuilding)
            Destroy(curPickObject);
        else if(state == BuyState.BuyBuilding)
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);

        if (buyState == BuyState.BuyOption)
            SetBuyOption(false, pickObject);
        else if (state == BuyState.BuyOption)
            SetBuyOption(true, pickObject);

        if (buyState == BuyState.SolveEvent)
            SetSolveEvent(false, pickObject);
        else if (state == BuyState.SolveEvent)
            SetSolveEvent(true, pickObject);

        if (buyState == BuyState.None)
            UIManager.instance.SetRoulettePopUp(false);
        else if (state == BuyState.None)
            UIManager.instance.SetRoulettePopUp(true);

        buyState = state;
    }

    public void BuyBuilding(Transform spawnTrans)
    {
        int cost = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost;
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyBuilding) return;
        if (!tile.CheckBuilding()) return;
        if (!PayMoney(cost)) return;

        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);
    }

    public void SellBuilding()
    {
        if (curPickObject == null) return;

        if (buyState != BuyState.SellBuilding) return;

        BuildingSpawner.instance.RemoveBuilding(curPickObject);
        Destroy(curPickObject);
    }

    public void BuyTile()
    {
        int cost = Grid.instance.tileCost;
        Tile tile = curPickObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyTile) return;
        if (!tile.CheckPurchased()) return;
        if (!PayMoney(cost)) return;

        tile.SetTilePurchased(true);
    }

    public bool BuyOption(OptionType type)
    {
        ResidentialBuilding building = curPickObject.GetComponent<ResidentialBuilding>();

        if (buyState != BuyState.BuyOption) return false;
        if (building.CheckFacility(type)) return false;
        if (!PayMoney(500)) return false;

        building.BuyFacility(type);
        return true;
    }

    public void SolveEvent(int index)
    {
        Building building = curPickObject.GetComponent<Building>();
        int cost = building.curEvents[index / 2].solutions[index % 2].cost;

        if (buyState != BuyState.SolveEvent) return;
        if (!PayMoney(cost)) return;

        building.SolveEvent(index);
        UIManager.instance.SetBuildingPopUp(true, curPickObject);
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
        {
            curPickObject.GetComponent<FollowMouse>().tile = tile.gameObject;
            SetObjectColor(curPickObject, Color.white);
        }
        else
        {
            SetObjectColor(curPickObject, Color.red);
            curPickObject.GetComponent<FollowMouse>().tile = null;
        }
    }

    public void SetObjectColor(GameObject obj, Color color)
    {
        if(obj.tag == "Tile")
        {
            if (obj.GetComponent<Tile>().CheckPurchased())
                obj.GetComponent<Tile>().SetTileColor(color);
            return;
        }

        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
                material.color = color;
        }
    }

    public void SetTargetObject(GameObject obj, Color targetColor, Color otherColor)
    {
        if (curPickObject != null)
        {
            SetObjectColor(curPickObject, otherColor);
        }

        if (obj != null)
        {
            SetObjectColor(obj, targetColor);
        }

        curPickObject = obj;
    }

    private void SetBuyOption(bool active, GameObject pickObject)
    {
        SetTargetObject(null, Color.red, Color.white);
        curPickObject = pickObject;
        UIManager.instance.SetOptionPopUp(active);
    }

    private void SetSolveEvent(bool active, GameObject pickObject = null)
    {
        curPickObject = pickObject;
        UIManager.instance.SetBuildingPopUp(active, pickObject);
    }
}
