using System.Collections;
using System.Collections.Generic;
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

    public override int CalculateBonus()
    {
        int income = 0;
        int cost = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 0;
            else
                income += 0;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                cost -= 100;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 0;
            else
                cost -= 50;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                cost -= 50;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 0;
            else
                income += 0;
        }

        bonusIncome += income;
        bonusCost += cost;

        return income + cost;
    }

    public override int UpdateHappiness(bool isExpect)
    {
        int changeAmount = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 3;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -2;
            else
                changeAmount += -1;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
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
                changeAmount += -1;
            else
                changeAmount += 2;
        }

        if (isExpect)
            return changeAmount;

        SetHappiness(changeAmount);
        return 0;
    }
}
