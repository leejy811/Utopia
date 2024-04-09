using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineManager : MonoBehaviour
{
    public static RoutineManager instance;

    public DateTime day;
    public float cityHappiness;
    public float cityHappinessDifference;

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
        day = new DateTime(2024, 1, 1);
    }

    public void DailyUpdate()
    {
        day.AddDays(1);

        Building.InitStaticCalcValue();
        CalculateBuilding();
        CalculateAdditional();

        if (BuildingSpawner.instance.buildings.Count >= 12)
        {
            EventManager.instance.CheckEvents();
            EventManager.instance.RandomRoulette();
        }
        UpdateHappiness();

        //Ελ°θ UI Set
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

        ShopManager.instance.GetMoney(total);
    }

    private void UpdateHappiness()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            building.happinessDifference = 0;
            building.UpdateHappiness();
            total += building.happinessRate;
        }

        EventManager.instance.EffectUpdate();

        if (BuildingSpawner.instance.buildings.Count != 0)
        {
            cityHappinessDifference = cityHappiness - total / BuildingSpawner.instance.buildings.Count;
        }
        else
            cityHappiness = 0;
    }

    public void SetCityHappiness(int happiness, int sign)
    {
        int count = BuildingSpawner.instance.buildings.Count;
        cityHappiness = ((cityHappiness * count) + happiness) / (count + sign);
        UIManager.instance.SetHappiness();
    }

    private void UpdateAfterStat()
    {
        ResidentialBuilding.cityResident -= ResidentialBuilding.residentReduction;
        cityHappiness -= cityHappinessDifference;
        UIManager.instance.UpdateDailyInfo();
    }
}
