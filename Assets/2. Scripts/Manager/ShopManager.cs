using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BuyState { None, BuyBuilding, SellBuilding, BuyTile, BuildTile, SolveBuilding, EventCheck }

public class ShopManager : MonoBehaviour, IObserver
{
    static public ShopManager instance;

    public BuyState buyState;

    [SerializeField] private int money;
    public int Money { get { return money; } set { money = value; UIManager.instance.Setmoney(); } }

    public GameObject[] buildingPrefabs;
    public GameObject[] tilePrefabs;

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
        Money = 500;
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
            UIManager.instance.SetErrorPopUp("!! ���� �����մϴ�.", pos);
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
        {
            curPickObject = Instantiate(buildingPrefabs[curPickIndex], transform);
            curPickObject.GetComponent<FollowMouse>().SetRedLineSize(curPickIndex);
            Grid.instance.SetTileColorMode();
        }

        if (buyState == BuyState.BuildTile)
            Destroy(curPickObject);
        else if (state == BuyState.BuildTile)
        {
            curPickObject = Instantiate(tilePrefabs[curPickIndex], transform);
            Grid.instance.SetTileColorMode();
        }

        if (buyState == BuyState.SolveBuilding)
            SetSolveEvent();
        else if (state == BuyState.SolveBuilding)
            SetSolveEvent(pickObject);

        if (buyState == BuyState.SolveBuilding && state == BuyState.BuyTile)
            curPickObject = null;

        if(buyState == BuyState.BuyTile)
            SetTargetObject(null, Color.green, Color.red);
        else if (buyState == BuyState.SellBuilding)
            SetTargetObject(null, Color.red, Color.white);

        switch (state)
        {
            case BuyState.None:
            case BuyState.SellBuilding:
            case BuyState.EventCheck:
                BuildingSpawner.instance.ChangeViewState(ViewStateType.Opaque);
                break;
            case BuyState.BuyBuilding:
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
        else if (buyState == BuyState.BuildTile)
        {
            Destroy(curPickObject);
            curPickObject = Instantiate(tilePrefabs[curPickIndex], transform);
        }
        else if (buyState == BuyState.SolveBuilding)
        {
            SetSolveEvent();
            SetSolveEvent(pickObject);
        }
        else if (buyState == BuyState.SellBuilding || buyState == BuyState.BuyTile)
        {
            ChangeState(BuyState.None);
        }
    }

    public void BuyBuilding(Transform spawnTrans)
    {
        int cost = CalculateCost(BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost);
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyBuilding) return;
        if (!tile.CheckBuildingWithErrorMsg()) return;
        if (!PayMoney(cost)) return;

        spawnTrans.rotation = curPickObject.transform.rotation;
        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);
    }

    public void SellBuilding()
    {
        if (curPickObject == null) return;

        if (buyState != BuyState.SellBuilding) return;

        BuildingSpawner.instance.RemoveBuilding(curPickObject);
        EventManager.instance.SetEventBuildings(curPickObject.GetComponent<Building>(), false);
        Destroy(curPickObject);
        AkSoundEngine.PostEvent("Play_Demolition_001_v1", gameObject);
    }

    public void BuyTile()
    {
        int cost = CalculateCost(Grid.instance.tileCost[0] * curPickTiles.Count);

        if (buyState != BuyState.BuyTile) return;
        if (cost == 0) return;
        if (!PayMoney(cost))
        {
            foreach (Vector2Int pos in curPickTiles)
            {
                Grid.instance.tiles[pos.x, pos.y].SetTileColor(false);
            }
            curPickTiles.Clear();
            return;
        }

        foreach(Vector2Int pos in curPickTiles)
        {
            Grid.instance.tiles[pos.x, pos.y].SetTilePurchased(true);
        }

        curPickTiles.Clear();
        AkSoundEngine.PostEvent("Play_Tile_Buy_001", gameObject);
    }

    public void BuildTile(Transform spawnTrans)
    {
        int cost = CalculateCost(Grid.instance.tileCost[curPickIndex + 1]);
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuildTile) return;
        if (!tile.CheckBuildingWithErrorMsg()) return;
        if (!PayMoney(cost)) return;

        Grid.instance.PlaceTile(curPickIndex, spawnTrans);
        
    }

    public void AddTile(Transform tile)
    {
        Vector2Int pos = new Vector2Int((int)tile.position.x, (int)tile.position.z);

        if (!curPickTiles.Contains(pos))
        {
            if (Grid.instance.tiles[pos.x, pos.y].CheckPurchased())
            {
                curPickTiles.Add(pos);
                Grid.instance.tiles[pos.x, pos.y].SetTileColor(true);
            }
        }
    }

    public bool BuyOption(OptionType type)
    {
        ResidentialBuilding building;
        if (buyState == BuyState.SolveBuilding)
            building = curPickObject.GetComponent<ResidentialBuilding>();
        else
            building = EventManager.instance.eventBuildings[UIManager.instance.eventNotify.curIndex] as ResidentialBuilding;

        if (buyState != BuyState.SolveBuilding && buyState != BuyState.EventCheck) return false;
        if (building.CheckFacility(type)) return false;
        if (!PayMoney(500)) return false;

        building.BuyFacility(type);

        return true;
    }

    public void SolveEvent(int index)
    {
        Building building;
        if (buyState == BuyState.SolveBuilding)
            building = curPickObject.GetComponent<Building>();
        else
            building = EventManager.instance.eventBuildings[UIManager.instance.eventNotify.curIndex];

        int cost = building.curEvents[index / 2].solutions[index % 2].cost;

        if (buyState != BuyState.SolveBuilding && buyState != BuyState.EventCheck) return;
        if (!PayMoney(cost)) return;

        building.SolveEvent(index);

        if (buyState == BuyState.SolveBuilding)
            UIManager.instance.SetBuildingIntroPopUp(building);
        else if(buyState == BuyState.EventCheck && building.GetEventProblemCount() != 0)
            UIManager.instance.SetEventNotifyValue(building);
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
        {
            curPickObject.GetComponent<FollowMouse>().tile = tile.gameObject;
            SetObjectColor(curPickObject, Color.white);

            int cost = 0;
            if (buyState == BuyState.BuyBuilding)
                cost = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>().cost;
            else if (buyState == BuyState.BuildTile)
                cost = Grid.instance.tileCost[curPickIndex + 1];
            UIManager.instance.SetCostPopUp(tile.transform, cost);
        }
        else
        {
            SetObjectColor(curPickObject, Color.red);
            curPickObject.GetComponent<FollowMouse>().tile = null;
            UIManager.instance.SetCostPopUp();
        }
    }

    public void SetObjectColor(GameObject obj, Color color)
    {
        if (obj == null) return;

        if (obj.tag == "Tile")
        {
            if (obj.GetComponent<Tile>().CheckPurchased())
            {
                bool isPurchased = color == Color.red ? false : true;
                obj.GetComponent<Tile>().SetTileColor(isPurchased);
            }
            return;
        }

        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
                material.color = new Color(color.r, color.g, color.b, material.color.a);
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
        {
            curPickObject = pickObject;
            UIManager.instance.SetBuildingIntroPopUp(pickObject.GetComponent<Building>());
            AkSoundEngine.SetRTPCValue("CLICK", 2);
        }
        else
        {
            UIManager.instance.SetBuildingIntroPopUp();
            AkSoundEngine.SetRTPCValue("CLICK", 1);
        }
    }

    private int CalculateCost(int cost)
    {
        foreach (Event globalEvent in EventManager.instance.globalEvents)
        {
            if(globalEvent.valueType == ValueType.Cost && globalEvent.targetIndex == (int)buyState)
            {
                cost += (int)(cost * (globalEvent.GetEffectValue(0) / 100.0f));
            }
        }

        return cost;
    }

    public void Notify(EventState state)
    {
        if (state != EventState.TileColor && state != EventState.EventNotify)
        {
            ChangeState(BuyState.None);
        }
    }

    public float GetDistToTarget(Transform transform)
    {
        if (buyState == BuyState.SolveBuilding)
        {
            float dist = Vector3.Distance(transform.position, curPickObject.transform.position);
            return dist;
        }
        else
            return 0.0f;
    }
}
