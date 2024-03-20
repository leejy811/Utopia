using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineManager : MonoBehaviour
{
    public int day;
    public int cityHappiness;

    public void DailyUpdate()
    {
        day++;

        UpdateHappiness();
        ShopManager.instance.GetMoney(CalculateTax());
        ShopManager.instance.money -= CalculateExpenditure();

        EventManager.instance.CheckEvents();
    }

    private int CalculateTax()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateIncome();

            if (building.type == BuildingType.Commercial && Convert.ToBoolean(building.CheckBonus()))
                total += 5;
            else if (building.type == BuildingType.culture && Convert.ToBoolean(building.CheckBonus()))
                total += 10;
        }

        return total;
    }

    private int CalculateExpenditure()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            if (building.type == BuildingType.Service)
                total += building.CalculateIncome();
            else if (building.type == BuildingType.Residential)
                total += building.CheckBonus() * 5;
        }

        return total;
    }

    private void UpdateHappiness()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            building.UpdateHappiness();
            total += building.happinessRate;
        }

        //ToDo
        //사회 현상에 의한 행복도 업데이트 구현

        cityHappiness = total / BuildingSpawner.instance.buildings.Count;
    }
}
