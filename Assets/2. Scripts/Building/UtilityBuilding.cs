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
        float tax = costPerDay * (happinessRate / 100.0f);
        tax += tax * GetIncomeEvent();
        int res = happinessRate >= 80 ? (int)(tax * 1.5f) : happinessRate < 20 ? (int)(tax * 0.5f) : (int)tax;

        if (type == BuildingType.Commercial)
            CommercialBuilding.income += res;
        else if (type == BuildingType.Culture)
            CultureBuilding.income += res;

        return res;
    }

    public override int CalculateBonus(bool isExpect = false)
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
        else if(values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 0;
            else
                cost -= 0;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                cost -= 0;
            else
                cost -= 0;
        }

        if (type == BuildingType.Commercial)
        {
            CommercialBuilding.bonusIncome += income;
            CommercialBuilding.bonusCost += cost;
        }
        else if (type == BuildingType.Culture)
        {
            CultureBuilding.bonusIncome += income;
            CultureBuilding.bonusCost += cost;
        }

        return income + cost;
    }

    public override int UpdateHappiness(bool isExpect = false, int valueType = 0)
    {
        int changeAmount = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -3;
            else
                changeAmount += -1;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -3;
            else
                changeAmount += -1;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 1;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -2;
            else
                changeAmount = 0;
        }

        if (isExpect)
            return changeAmount;

        SetHappiness(changeAmount);
        return 0;
    }

    public override void ApplyInfluence(int value, BuildingType type, bool isInit = false)
    {
        if (type != BuildingType.Residential) return;

        BoundaryValue cast = values[ValueType.user];
        cast.cur += value;
        values[ValueType.user] = cast;

        if (!isInit)
        {
            int amount = UpdateHappiness(true);
            UIManager.instance.SetHappinessPopUp(amount, transform.position);
        }
    }
}
