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
    public ResultType weekResult;
    public int[] debtsOfWeek;
    public int debt;
    public int creditRating;
    public int maxCreditRating;

    public bool isPay;

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
        day = new DateTime(2023, 12, 31);
        isPay = true;
    }

    public void DailyUpdate()
    {
        day = day.AddDays(1);

        Building.InitStaticCalcValue();
        Tile.income = 0;
        CalculateIncome();
        UpdateHappiness();
        EventManager.instance.EffectUpdate();

        ApplyDept();
    }

    private void ApplyDept()
    {
        if (GetWeekOfYear() >= debtsOfWeek.Length) 
        {
            UIManager.instance.SetDebtSlider(0);
            return;
        }

        DayOfWeek dayOfWeek = day.DayOfWeek;

        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                CalculateDept();
                break;
            case DayOfWeek.Thursday:
                if (!isPay)
                    UIManager.instance.notifyObserver(EventState.Reminder);
                break;
        }

        int week = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        UIManager.instance.SetDebtSlider(week);
    }

    private void CalculateDept()
    {
        if (!isPay)
        {
            SetCreditRating(1);
            weekResult = ResultType.Worst;
            UIManager.instance.notifyObserver(EventState.LateReceipt);
            debt = debtsOfWeek[GetWeekOfYear()];
            return;
        }

        debt = debtsOfWeek[GetWeekOfYear()];
        isPay = false;

        UIManager.instance.OnClickDeptDocButton();
    }

    private int GetWeekOfYear()
    {
        int weekOfYear = (day.DayOfYear / 7) + (creditRating * -1) + ((day.Year - 2024) * 52);
        return weekOfYear;
    }

    private void SetCreditRating(int value)
    {
        creditRating += value;
        UIManager.instance.SetCreditRating();

        if(creditRating > maxCreditRating)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
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

        ResidentialBuilding.yesterDayResident = ResidentialBuilding.cityResident;

        AkSoundEngine.SetRTPCValue("HAPPY", cityHappiness);
    }

    public void SetCityHappiness(int happiness, int sign)
    {
        int count = BuildingSpawner.instance.buildings.Count;

        cityHappiness = count + sign == 0 ? 0 : ((cityHappiness * count) + happiness) / (count + sign);
        UIManager.instance.SetHappiness();
        AkSoundEngine.SetRTPCValue("HAPPY", cityHappiness);
    }

    public void PayDept()
    {
        DayOfWeek dayOfWeek = day.DayOfWeek;
        int dayOfWeekToInt = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        int money = ShopManager.instance.Money;

        if (isPay) return;
        if (money < debt) return;

        UIManager.instance.notifyObserver(EventState.Receipt);
        ShopManager.instance.PayMoney(debt);
        debt = 0;
        UIManager.instance.SetDebt();

        if (dayOfWeekToInt < (int)DayOfWeek.Thursday)
        {
            RewardToPrePay();
        }

        switch(dayOfWeek)
        {
            case DayOfWeek.Monday:
                weekResult = ResultType.Perfect;
                break;
            case DayOfWeek.Tuesday:
                weekResult = ResultType.Best;
                break;
            case DayOfWeek.Wednesday:
                weekResult = ResultType.Good;
                break;
            case DayOfWeek.Thursday:
            case DayOfWeek.Friday:
            case DayOfWeek.Saturday:
                weekResult = ResultType.Soso;
                break;
            case DayOfWeek.Sunday:
                weekResult = ResultType.Bad;
                break;
        }

        isPay = true;

        int weekOfYear = (day.DayOfYear / 7) + 1 + (creditRating * -1) + ((day.Year - 2024) * 52);
        CityLevelManager.instance.UpdateCityType(weekOfYear);
    }

    private void RewardToPrePay()
    {
        int rewardHappiness = 5;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            building.SetHappiness(rewardHappiness);
        }
    }
}
