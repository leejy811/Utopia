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
    public int fee;
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
        if (!GameManager.instance.isLoad)
        {
            InitChipCost();
        }
    }

    public void InitChipCost()
    {
        CostUpdate();
        RatioUpdate(true);

        if(GameManager.instance.isLoad)
        {
            ChipData data = DataBaseManager.instance.LoadChipData();
            chipCostDatas = data.Load();
            changeRatio = data.changeRatio;
            curChip = data.curChip;
        }
        else
        {
            chipCostDatas[new DateTime(2023, 12, 31)] = baseCost / 2;
        }
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

    public void RatioUpdate(bool isInit = false)
    {
        float ranNum = UnityEngine.Random.Range(0.0f, 1.0f);

        if (ranNum < increaseProb)
            changeRatio = UnityEngine.Random.Range(increaseRange.x, increaseRange.y);
        else
            changeRatio = UnityEngine.Random.Range(decreaseRange.x, decreaseRange.y);

        chipCostDatas[RoutineManager.instance.day] = CalcChipCost();
        if (!isInit)
            DataBaseManager.instance.SaveChipData(chipCostDatas, changeRatio, curChip);
    }

    public int CalcChipCost()
    {
        return (int)((baseCost * (1.0f + changeRatio)) / 2);
    }

    public int CalcChipCostWithFee(int amount)
    {
        int cost = CalcChipCost() * amount;
        cost += (int)Mathf.Abs(cost * (GetFee() / 100.0f));
        return cost;
    }

    public void TradeChip(int amount)
    {
        int totalCost = CalcChipCostWithFee(amount);

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

    public int GetFee()
    {
        int fee = this.fee;
        foreach (Event e in EventManager.instance.globalEvents)
        {
            if (e.valueType == ValueType.Chip)
                fee = fee + e.effectValue[0];
        }
        return fee;
    }
}
