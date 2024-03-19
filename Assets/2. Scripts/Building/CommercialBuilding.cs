using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CommercialBuilding : Building
{
    public BoundaryValue customerCnt;
    public BoundaryValue productPrice;

    public int income;

    public override int CalculateIncome()
    {
        int res = income * happinessRate;
        return happinessRate >= 80 ? (int)(res * 1.5f) : happinessRate < 20 ? (int)(res * 0.5f) : res;
    }
    public override int CheckBonus()
    {
        int res = productPrice.cur > productPrice.max ? 1 : 0;
        return res;
    }

    public override void UpdateHappiness()
    {
        //customerCnt
        if (customerCnt.cur > customerCnt.max)
            happinessRate += 2;
        else if (customerCnt.cur < customerCnt.min)
            happinessRate -= 2;

        //productPrice
        if (productPrice.cur > productPrice.max)
        {
            happinessRate += 2;
            ShopManager.instance.money += 5;
        }
        else if (productPrice.cur < productPrice.min)
            happinessRate -= 1;
    }

}
