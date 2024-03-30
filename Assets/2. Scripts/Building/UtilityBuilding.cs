using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UtilityBuilding : Building
{
    public int costPerDay;

    public BoundaryValue userCnt;
    public BoundaryValue utilityValue;

    private void Awake()
    {
        values[ValueType.user] = userCnt;
        values[ValueType.utility] = utilityValue;
    }

    public override int CalculateIncome()
    {
        int res = costPerDay * happinessRate;
        return happinessRate >= 80 ? (int)(res * 1.5f) : happinessRate < 20 ? (int)(res * 0.5f) : res;
    }

    public override int CalculateBonus()
    {
        int res = 0;
        
        if(values[ValueType.user].cur > values[ValueType.user].max)
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                res += 100;
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                res += 0;
            else
                res += 50;
        }
        else if(values[ValueType.user].cur < values[ValueType.user].min)
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                res += 0;
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                res -= 100;
            else
                res -= 50;
        }
        else
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                res += 50;
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                res -= 50;
            else
                res += 0;
        }

        return res;
    }

    public override void UpdateHappiness()
    {
        if (values[ValueType.user].cur > values[ValueType.user].max)
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                SetHappiness(2);
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                SetHappiness(0);
            else
                SetHappiness(1);
        }
        else if (values[ValueType.user].cur < values[ValueType.user].min)
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                SetHappiness(0);
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                SetHappiness(-2);
            else
                SetHappiness(-1);
        }
        else
        {
            if (values[ValueType.user].cur > values[ValueType.user].max)
                SetHappiness(1);
            else if (values[ValueType.user].cur < values[ValueType.user].min)
                SetHappiness(-1);
            else
                SetHappiness(0);
        }
    }
}
