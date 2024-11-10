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

    public virtual void SetParameter(int sign)
    {
        BoundaryValue utility = values[ValueType.utility];
        utility.cur = GetRandomParameter(sign);
        values[ValueType.utility] = utility;
    }

    public virtual float GetRandomParameter(int sign)
    {
        return utilityValue.cur + (utilityValue.cur * 0.25f * sign);
    }

    public override int CalculateIncome()
    {
        float tax = costPerDay * (happinessRate / 100.0f);
        tax += tax * GetIncomeEvent();
        int res = happinessRate >= 80 ? (int)(tax * 1.5f) : happinessRate < 20 ? (int)(tax * 0.5f) : (int)tax;

        if (type == BuildingType.Commercial)
            CommercialBuilding.income += res;
        else if (type == BuildingType.Culture)
        {
            switch (GameManager.instance.curMapType)
            {
                case MapType.Utopia:
                    CultureBuilding.income += res;
                    break;
                case MapType.Totopia:
                    EnterBuilding.income += res;
                    break;
            }
        }

        return res;
    }

    public override int CalculateBonus(bool isExpect = false, int valueType = 0)
    {
        int income = 0;
        int cost = 0;

        if (values[ValueType.user].CheckBoundary() == BoundaryType.More)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 15;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 5;
            else
                income += 10;
        }
        else if(values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += -5;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += 1;
            else
                income += -10;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                income += 5;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                income += -5;
            else
                income += 5;
        }

        if (!isExpect)
        {
            if (type == BuildingType.Commercial)
            {
                CommercialBuilding.bonusIncome += income;
                CommercialBuilding.bonusCost += cost;
            }
            else if (type == BuildingType.Culture)
            {
                switch (GameManager.instance.curMapType)
                {
                    case MapType.Utopia:
                        CultureBuilding.bonusIncome += income;
                        CultureBuilding.bonusCost += cost;
                        break;
                    case MapType.Totopia:
                        EnterBuilding.bonusIncome += income;
                        EnterBuilding.bonusCost += cost;
                        break;
                }
            }
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
                changeAmount += -10;
            else
                changeAmount += 0;
        }
        else if (values[ValueType.user].CheckBoundary() == BoundaryType.Less)
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += -20;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += 15;
            else
                changeAmount += -20;
        }
        else
        {
            if (values[ValueType.utility].CheckBoundary() == BoundaryType.More)
                changeAmount += 0;
            else if (values[ValueType.utility].CheckBoundary() == BoundaryType.Less)
                changeAmount += -20;
            else
                changeAmount += 10;
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
            ExpectHappiness(0);
        }
    }
}
