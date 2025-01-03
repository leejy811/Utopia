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

    public override int CalculateBonus(bool isExpect = false, int valueType = 0)
    {
        int income = 0;
        int cost = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 10;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 5;
            else
                income += 5;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 10;
            else
                cost -= 5;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 5;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 0;
            else
                income += 10;
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
                changeAmount += 10;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -20;
            else
                changeAmount += -20;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += 10;
            else
                changeAmount += -10;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -10;
            else
                changeAmount += 10;
        }

        if (isExpect)
            return changeAmount;

        SetHappiness(changeAmount);
        return 0;
    }
}
