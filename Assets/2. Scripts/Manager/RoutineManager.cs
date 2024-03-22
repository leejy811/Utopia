using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineManager : MonoBehaviour
{
    public static RoutineManager instance;

    public int day;
    public int cityHappiness;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        day++;
    }

    public void DailyUpdate()
    {
        day++;

        EventManager.instance.CheckEvents();
        EventManager.instance.RandomRoulette();
        UpdateHappiness();

        ShopManager.instance.GetMoney(CalculateTax());
        ShopManager.instance.money -= CalculateExpenditure();

        UIManager.instance.UpdateDailyInfo();
    }

    private int CalculateTax()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateIncome();

            if (building.type == BuildingType.Commercial && Convert.ToBoolean(building.CheckBonus()))
                total += 5;
            else if (building.type == BuildingType.Culture && Convert.ToBoolean(building.CheckBonus()))
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

        EventManager.instance.EffectUpdate();

        if (BuildingSpawner.instance.buildings.Count != 0)
            cityHappiness = total / BuildingSpawner.instance.buildings.Count;
        else
            cityHappiness = 0;
    }
}
