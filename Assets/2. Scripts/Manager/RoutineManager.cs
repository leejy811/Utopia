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
        CalculateIncome();
        UpdateHappiness();
    }

    private void CalculateIncome()
    {
        int total = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateIncome() + building.CalculateBonus();
        }

        total += (int)(Mathf.Abs(total) * EventManager.instance.GetFinalIncomeEventValue());
        
        ShopManager.instance.GetMoney(total);
    }

    private void CalculateBonus()
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

        if (BuildingSpawner.instance.buildings.Count != 0)
        {
            cityHappinessDifference = total / BuildingSpawner.instance.buildings.Count - cityHappiness;
            cityHappiness += cityHappinessDifference;
        }
        else
            cityHappiness = 0;
    }

    public void SetCityHappiness(int happiness, int sign)
    {
        int count = BuildingSpawner.instance.buildings.Count;

        cityHappiness = count + sign == 0 ? 0 : ((cityHappiness * count) + happiness) / (count + sign);
        UIManager.instance.SetHappiness();
        AkSoundEngine.SetRTPCValue("HAPPY", cityHappiness);
    }

    public void UpdateAfterStat()
    {
        ResidentialBuilding.yesterDayResident = ResidentialBuilding.cityResident;

        EventManager.instance.CheckEvents();
        EventManager.instance.EffectUpdate();
    }
}
