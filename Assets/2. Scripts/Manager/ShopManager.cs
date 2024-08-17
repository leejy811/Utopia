using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
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
            string[] str = { "!! 돈이 부족합니다." };
            UIManager.instance.SetErrorPopUp(str, pos);
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

        if (buyState == BuyState.SolveBuilding)
            curPickObject = null;

        if (buyState == BuyState.SellBuilding)
            SetTargetObject(null, Color.red, Color.white);

        switch (state)
        {
            case BuyState.None:
            case BuyState.SellBuilding:
            case BuyState.EventCheck:
                BuildingSpawner.instance.ChangeViewState(ViewStateType.Opaque);
                break;
            case BuyState.BuyBuilding:
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
        else if (buyState == BuyState.SellBuilding)
        {
            ChangeState(BuyState.None);
        }
    }

    public float CalculateBuildingCostWeight(int index)
    {
        float coefficient = 0.5f;
        float baseValue = 1.08f;
        
        return coefficient * (Mathf.Pow(baseValue, (BuildingSpawner.instance.buildingCount[index]))-1);
    }

    public void BuyBuilding(Transform spawnTrans)
    {
        Building building = BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>();
        int cost = CalculateBuildingCost(building, curPickIndex);

        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuyBuilding) return;
        if (!tile.CheckBuildingWithErrorMsg()) return;
        if (!PayMoney(cost)) return;

        spawnTrans.rotation = curPickObject.transform.rotation;
        BuildingSpawner.instance.PlaceBuilding(curPickIndex, spawnTrans);

        UIManager.instance.SetBuildingListValue((int)building.type);
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

    public void BuildTile(Transform spawnTrans)
    {
        int cost = CalculateCost(Grid.instance.tileCost[curPickIndex + 1]);
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();

        if (buyState != BuyState.BuildTile) return;
        if (!tile.CheckBuildingWithErrorMsg()) return;
        if (!PayMoney(cost)) return;

        Grid.instance.PlaceTile(curPickIndex, spawnTrans);
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
        if (!PayMoney(150)) return false;

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
        else if (buyState == BuyState.EventCheck)
        {
            if (building.GetEventProblemCount() != 0)
                UIManager.instance.SetEventNotifyValue(building);
            else if (EventManager.instance.eventBuildings.Count != 0)
                UIManager.instance.SetEventNotifyValue(EventManager.instance.eventBuildings[UIManager.instance.eventNotify.curIndex]);
        }
    }

    public void CheckBuyBuilding(Transform tileTrans)
    {
        Tile tile = tileTrans.gameObject.GetComponent<Tile>();
        if (tile.CheckBuilding())
        {
            curPickObject.GetComponent<FollowMouse>().tile = tile.gameObject;
            curPickObject.GetComponent<FollowMouse>().SetRedLineSize(curPickIndex);
            SetObjectColor(curPickObject, Color.white);

            int cost = 0;
            if (buyState == BuyState.BuyBuilding)
            {
                cost = CalculateBuildingCost(BuildingSpawner.instance.buildingPrefabs[curPickIndex].GetComponent<Building>(), curPickIndex);
            }
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
            pickObject.GetComponent<Building>().SetActiveInfluenceObject(true);
            AkSoundEngine.SetRTPCValue("CLICK", 2);
        }
        else
        {
            curPickObject.GetComponent<Building>().SetActiveInfluenceObject(false);
            UIManager.instance.SetBuildingIntroPopUp();
            AkSoundEngine.SetRTPCValue("CLICK", 1);
        }
    }

    private int CalculateCost(int cost)
    {
        foreach (Event globalEvent in EventManager.instance.globalEvents)
        {
            if (globalEvent.valueType == ValueType.Cost && globalEvent.targetIndex == (int)buyState)
            {
                cost += (int)(cost * (globalEvent.GetEffectValue(0) / 100.0f));
            }
        }

        return cost;
    }

    public int CalculateBuildingCost(Building building, int index)
    {
        float costWeight = CalculateBuildingCostWeight(index);
        int originalCost = building.cost;
        return CalculateCost(Mathf.RoundToInt(Mathf.RoundToInt(originalCost + (originalCost * costWeight)) / 10.0f) * 10);
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
