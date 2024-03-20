using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CommercialBuilding : Building
{
    public BoundaryValue customerCnt;
    public BoundaryValue productPrice;

    public int income;

    private void Awake()
    {
        values[ValueType.Customer] = customerCnt;
        values[ValueType.Product] = productPrice;
    }

    public override int CalculateIncome()
    {
        int res = income * happinessRate;
        return happinessRate >= 80 ? (int)(res * 1.5f) : happinessRate < 20 ? (int)(res * 0.5f) : res;
    }
    public override int CheckBonus()
    {
        int res = values[ValueType.Product].cur > values[ValueType.Product].max ? 1 : 0;
        return res;
    }

    public override void UpdateHappiness()
    {
        //customerCnt
        if (values[ValueType.Customer].cur > values[ValueType.Customer].max)
            happinessRate += 2;
        else if (values[ValueType.Customer].cur < values[ValueType.Customer].min)
            happinessRate -= 2;

        //productPrice
        if (values[ValueType.Product].cur > values[ValueType.Product].max)
        {
            happinessRate += 2;
            ShopManager.instance.money += 5;
        }
        else if (values[ValueType.Product].cur < values[ValueType.Product].min)
            happinessRate -= 1;
    }

}
