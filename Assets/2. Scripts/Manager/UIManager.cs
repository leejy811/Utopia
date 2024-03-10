using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnClickBuildingBuy(int index)
    {
        if (BuildingSpawner.instance.isBuyBuilding) return;
        if (!PlayerInfo.instance.PayMoney(10))
            return;

        BuildingSpawner.instance.BuyBuilding(index);
    }
}
