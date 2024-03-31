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
        
        if(values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                res += 100;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                res += 0;
            else
                res += 50;
        }
        else if(values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                res += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                res -= 100;
            else
                res -= 50;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                res += 50;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                res -= 50;
            else
                res += 0;
        }

        return res;
    }

    public override void UpdateHappiness()
    {
        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                SetHappiness(2);
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                SetHappiness(0);
            else
                SetHappiness(1);
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                SetHappiness(0);
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                SetHappiness(-2);
            else
                SetHappiness(-1);
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                SetHappiness(1);
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                SetHappiness(-1);
            else
                SetHappiness(0);
        }
    }

    public override void ApplyInfluence(int value, BuildingType type = 0)
    {
        BoundaryValue cast = values[ValueType.user];
        cast.cur += value;
        values[ValueType.user] = cast;
    }
}
