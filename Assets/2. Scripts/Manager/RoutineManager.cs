using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType { Worst, Bad, Soso, Good, Best, Perfect }

public class RoutineManager : MonoBehaviour
{
    public static RoutineManager instance;

    public DateTime day;
    public float cityHappiness;
    public float cityHappinessDifference;
    public ResultType todayResult;

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
        Tile.income = 0;
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

        for(int i = 0;i < Grid.instance.width;i++)
        {
            for (int j = 0; j < Grid.instance.height; j++)
            {
                total += Grid.instance.tiles[i, j].CaculateIncome();
            }
        }

        total = (int)(total * EventManager.instance.GetFinalIncomeEventValue());
        
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

        int happiness = (int)cityHappiness;

        if (happiness < 30)
            todayResult = ResultType.Worst;
        else if (happiness >= 30 && happiness < 40)
            todayResult = ResultType.Bad;
        else if (happiness >= 40 && happiness < 55)
            todayResult = ResultType.Soso;
        else if (happiness >= 55 && happiness < 65)
            todayResult = ResultType.Good;
        else if (happiness >= 65 && happiness < 70)
            todayResult = ResultType.Best;
        else if (happiness >= 70)
            todayResult = ResultType.Perfect;
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
