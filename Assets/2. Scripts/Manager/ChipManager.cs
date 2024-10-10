using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipManager : MonoBehaviour
{
    static public ChipManager instance;

    public int curTradeTimes;
    public int maxTradeTimes;
    public int curChip;
    public int baseCost;
    public float changeRatio;
    public Vector2 increaseRange;
    public Vector2 decreaseRange;
    [Range(0.0f, 1.0f)]public float increaseProb;

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
        float ranNum = Random.Range(0.0f, 1.0f);

        if (ranNum < increaseProb)
            changeRatio = Random.Range(increaseRange.x, increaseRange.y);
        else
            changeRatio = Random.Range(decreaseRange.x, decreaseRange.y);
    }

    public int CalcChipCost()
    {
        return (int)(baseCost * (1.0f + changeRatio));
    }

    public void TradeChip(int amount)
    {
        curTradeTimes--;
        curChip += amount;
    }

    public bool PayChip(int amount)
    {
        if(curChip - amount < 0)
            return false;

        curChip -= amount;

        return true;
    }
}
