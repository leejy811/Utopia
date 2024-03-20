using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CultureBilding : Building
{
    public BoundaryValue trendValue;
    public BoundaryValue fee;

    public int income;

    private void Awake()
    {
        values[ValueType.Trend] = trendValue;
        values[ValueType.Fee] = fee;
    }

    public override int CalculateIncome()
    {
        int res = income * happinessRate;
        return happinessRate >= 80 ? (int)(res * 1.5f) : happinessRate < 20 ? (int)(res * 0.5f) : res;
    }

    public override int CheckBonus()
    {
        int res = values[ValueType.Fee].cur > values[ValueType.Fee].max ? 1 : 0;
        return res;
    }

    public override void UpdateHappiness()
    {
        //trendValue
        if (values[ValueType.Trend].cur > values[ValueType.Trend].max)
        {
            happinessRate += 2;
            //ToDo
            //영향력 범위 늘리기 추가
        }
        else if (values[ValueType.Trend].cur < values[ValueType.Trend].min)
            happinessRate -= 2;

        //fee
        if (values[ValueType.Fee].cur > values[ValueType.Fee].max)
        {
            happinessRate += 2;
            ShopManager.instance.money += 10;
        }
        else if (values[ValueType.Fee].cur < values[ValueType.Fee].min)
            happinessRate -= 1;
    }
}
