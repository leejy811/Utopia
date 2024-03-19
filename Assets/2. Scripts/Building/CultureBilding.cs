using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultureBilding : Building
{
    public BoundaryValue trendValue;
    public BoundaryValue fee;

    public int income;

    public override int CalculateIncome()
    {
        int res = income * happinessRate;
        return happinessRate >= 80 ? (int)(res * 1.5f) : happinessRate < 20 ? (int)(res * 0.5f) : res;
    }

    public override int CheckBonus()
    {
        int res = fee.cur > fee.max ? 1 : 0;
        return res;
    }

    public override void UpdateHappiness()
    {
        //trendValue
        if (trendValue.cur > trendValue.max)
        {
            happinessRate += 2;
            //ToDo
            //영향력 범위 늘리기 추가
        }
        else if (trendValue.cur < trendValue.min)
            happinessRate -= 2;

        //fee
        if (fee.cur > fee.max)
        {
            happinessRate += 2;
            ShopManager.instance.money += 10;
        }
        else if (fee.cur < fee.min)
            happinessRate -= 1;
    }
}
