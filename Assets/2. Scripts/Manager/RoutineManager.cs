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
        day = day.AddDays(1);

        Building.InitStaticCalcValue();
        CalculateBuilding();
        CalculateAdditional();
        UpdateHappiness();
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
            building.UpdateHappiness(false);
            total += building.happinessRate;
        }

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

        cityHappiness = count + sign == 0 ? 0 : ((cityHappiness * count) + happiness) / (count + sign);
        UIManager.instance.SetHappiness();
    }

    public void UpdateAfterStat()
    {
        ResidentialBuilding.yesterDayResident = ResidentialBuilding.cityResident;
        cityHappiness -= cityHappinessDifference;

        EventManager.instance.CheckEvents();
        EventManager.instance.EffectUpdate();
    }
}
