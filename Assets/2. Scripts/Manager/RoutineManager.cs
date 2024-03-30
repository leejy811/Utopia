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

        CalculateBuilding();
        CalculateAdditional();

        if (BuildingSpawner.instance.buildings.Count >= 12)
        {
            EventManager.instance.CheckEvents();
            EventManager.instance.RandomRoulette();
        }
        UpdateHappiness();

        UIManager.instance.UpdateDailyInfo();
    }

    private void CalculateBuilding()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateIncome();
        }

        ShopManager.instance.GetMoney(total);
    }

    private void CalculateAdditional()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateBonus();
        }
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
