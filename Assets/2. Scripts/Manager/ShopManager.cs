using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BuyState { None, BuyBuilding, SellBuilding, BuyTile, BuildTile, SolveBuilding }

public class ShopManager : MonoBehaviour
{
    static public ShopManager instance;

    public BuyState buyState;
    private int money;
    public int Money { get { return money; } set { money = value; UIManager.instance.Setmoney(); } }

    public GameObject[] buildingPrefabs;

    private GameObject curPickObject;
    private int curPickIndex;

    public List<Vector2Int> curPickTiles = new List<Vector2Int>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Money = 10000;
    }

    public void GetMoney(int amount)
    {
        Money += amount;
    }

    public bool PayMoney(int amount)
    {
        if (Money - amount < 0)
        {
            Vector3 pos = curPickObject == null ? Vector3.zero : curPickObject.transform.position;
            UIManager.instance.SetErrorPopUp("!! 돈이 부족합니다.", pos);
            return false;
        }

        Money -= amount;
        return true;
    }

    public void ChangeState(BuyState state, int index = 0, GameObject pickObject = null)
    {
        if (state == buyState)
        {
            ChangePickObject(index, pickObject);
            return;
        }

        curPickIndex = index;

        if (buyState == BuyState.BuyBuilding)
            Destroy(curPickObject);
        else if(state == BuyState.BuyBuilding)
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);

        if (buyState == BuyState.SolveBuilding)
            SetSolveEvent();
        else if (state == BuyState.SolveBuilding)
            SetSolveEvent(pickObject);

        if (buyState == BuyState.None)
            UIManager.instance.SetRoulettePopUp(false);
        else if (state == BuyState.None)
            UIManager.instance.SetRoulettePopUp(true);

        switch (state)
        {
            case BuyState.None:
            case BuyState.SellBuilding:
                BuildingSpawner.instance.ChangeViewState(ViewStateType.Opaque);
                break;
            case BuyState.BuyBuilding:
                BuildingSpawner.instance.ChangeViewState(ViewStateType.Translucent);
                break;
            case BuyState.BuyTile:
            case BuyState.BuildTile:
                BuildingSpawner.instance.ChangeViewState(ViewStateType.Transparent);
                break;
        }

        buyState = state;
    }

    public void ChangePickObject(int index = 0, GameObject pickObject = null)
    {
        curPickIndex = index;

        if (buyState == BuyState.BuyBuilding)
        {
            Destroy(curPickObject);
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);
        }
        else if (buyState == BuyState.SolveBuilding)
        {
            SetSolveEvent();
            SetSolveEvent(pickObject);
        }
    }

    public void BuyBuilding(Transform spawnTrans)
    {
        int cost = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost;
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyBuilding) return;
        if (!tile.CheckBuilding()) return;
        if (!PayMoney(cost)) return;

        spawnTrans.rotation = curPickObject.transform.rotation;
        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);
    }

    public void SellBuilding()
    {
        if (curPickObject == null) return;

        if (buyState != BuyState.SellBuilding) return;

        BuildingSpawner.instance.RemoveBuilding(curPickObject);
        UIManager.instance.SetBuildingIntroPopUp();
        Destroy(curPickObject);
    }

    public void BuyTile()
    {
        int cost = Grid.instance.tileCost * curPickTiles.Count;

        if (buyState != BuyState.BuyTile) return;
        if (!PayMoney(cost)) return;

        foreach(Vector2Int pos in curPickTiles)
        {
            Grid.instance.tiles[pos.x, pos.y].SetTilePurchased(true);
        }

        curPickTiles.Clear();
    }

    public void AddTile(Transform tile)
    {
        Vector2Int pos = new Vector2Int((int)tile.position.x, (int)tile.position.z);

        if (!curPickTiles.Contains(pos))
        {
            if (Grid.instance.tiles[pos.x, pos.y].CheckPurchased())
            {
                curPickTiles.Add(pos);
                Grid.instance.tiles[pos.x, pos.y].SetTileColor(Color.green);
            }
        }
    }

    public bool BuyOption(OptionType type)
    {
        ResidentialBuilding building = curPickObject.GetComponent<ResidentialBuilding>();

        if (buyState != BuyState.SolveBuilding) return false;
        if (building.CheckFacility(type)) return false;
        if (!PayMoney(500)) return false;

        building.BuyFacility(type);
        return true;
    }

    public void SolveEvent(int index)
    {
        Building building = curPickObject.GetComponent<Building>();
        int cost = building.curEvents[index / 2].solutions[index % 2].cost;

        if (buyState != BuyState.SolveBuilding) return;
        if (!PayMoney(cost)) return;

        building.SolveEvent(index);
        UIManager.instance.SetBuildingIntroPopUp(building);
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
        {
            curPickObject.GetComponent<FollowMouse>().tile = tile.gameObject;
            SetObjectColor(curPickObject, Color.white);

            //ToDo 건설 비용 표시
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

    public void RotatePickBuilding()
    {
        curPickObject.transform.Rotate(Vector3.up * 90f);
    }

    private void SetSolveEvent(GameObject pickObject = null)
    {
        if (pickObject != null)
            curPickObject = pickObject;
        UIManager.instance.SetBuildingIntroPopUp(pickObject.GetComponent<Building>());
    }
}
