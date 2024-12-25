using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipManager : MonoBehaviour
{
    static public ChipManager instance;

    public int curChip;
    public int baseCost;
    public float changeRatio;
    public Vector2 increaseRange;
    public Vector2 decreaseRange;
    [Range(0.0f, 1.0f)]public float increaseProb;
    public Dictionary<DateTime, int> chipCostDatas = new Dictionary<DateTime, int>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        CostUpdate();
        RatioUpdate();

        chipCostDatas[new DateTime(2023, 12, 31)] = baseCost / 2;
    }

    public void CostUpdate()
    {
        baseCost = 0;
        int count = 0;
        for (int i = 0;i < BuildingSpawner.instance.buildingPrefabs.Length;i++)
        {
            Building building = BuildingSpawner.instance.buildingPrefabs[i].GetComponent<Building>();
            if (!CityLevelManager.instance.CheckBuildingLevel(building)) return;

            baseCost += ShopManager.instance.CalculateBuildingCost(building, i);
            count++;
        }

        baseCost /= count;
    }

    public void RatioUpdate()
    {
        float ranNum = UnityEngine.Random.Range(0.0f, 1.0f);

        if (ranNum < increaseProb)
            changeRatio = UnityEngine.Random.Range(increaseRange.x, increaseRange.y);
        else
            changeRatio = UnityEngine.Random.Range(decreaseRange.x, decreaseRange.y);

        chipCostDatas[RoutineManager.instance.day] = CalcChipCost();
    }

    public int CalcChipCost()
    {
        return (int)((baseCost * (1.0f + changeRatio)) / 2);
    }

    public int CalcChipCostWithEvent()
    {
        int cost = (int)((baseCost * (1.0f + changeRatio)) / 2);

        foreach (Event e in EventManager.instance.globalEvents)
        {
            if (e.valueType == ValueType.Chip)
                cost = (int)(cost * ((e.effectValue[0] + 100.0f) / 100.0f));
        }

        return cost;
    }

    public void TradeChip(int amount)
    {
        int totalCost = amount * CalcChipCost();

        foreach (Event e in EventManager.instance.globalEvents)
        {
            if (e.valueType == ValueType.Chip)
                totalCost = (int)(totalCost * ((e.effectValue[0] + 100.0f) / 100.0f));
        }

        curChip += amount;
        ShopManager.instance.GetMoney(-totalCost);
    }

    public bool PayChip(int amount)
    {
        if(curChip - amount < 0)
            return false;

        curChip -= amount;

        return true;
    }
}
