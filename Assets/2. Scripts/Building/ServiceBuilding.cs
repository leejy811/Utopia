using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ServiceBuilding : UtilityBuilding
{
    public static int income;
    public static int bonusIncome;
    public static int bonusCost;
    public override int CalculateIncome()
    {
        float tax = costPerDay * (1.0f - (happinessRate / 100.0f)) * -1;
        tax += tax * GetIncomeEvent();
        int res = happinessRate < 40 ? ((int)(tax * 1.5f)) : happinessRate >= 80 ? ((int)(tax * 0.5f)) : (int)tax;
        income += res;
        return res;
    }

    public override int CalculateBonus(bool isExpect = false)
    {
        int income = 0;
        int cost = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 1;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 10;
            else
                cost -= 5;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 10;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 1;
            else
                income += 5;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 5;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 5;
            else
                income += 1;
        }

        if (!isExpect)
        {
            bonusIncome += income;
            bonusCost += cost;
        }

        return income + cost;
    }

    public override int UpdateHappiness(bool isExpect = false, int valueType = 0)
    {
        int changeAmount = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 2;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += 0;
            else
                changeAmount += 0;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += -1;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += 2;
            else
                changeAmount += 0;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += 1;
            else
                changeAmount += 2;
        }

        if (isExpect)
            return changeAmount;

        SetHappiness(changeAmount);
        return 0;
    }
}
