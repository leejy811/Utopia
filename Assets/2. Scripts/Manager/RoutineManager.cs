using DG.Tweening;
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

    public Light mainLight;
    public float lightUpdateDuration;
    public float lightDailySpeed;
    public float defalutAngleX;

    private Coroutine lightCoroutine;
    private int totalIncome;

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
        debt = debtsOfWeek[GetWeekOfYear()];
        isPay = false;
        lightCoroutine = StartCoroutine(DailyLight());
    }

    public void DailyUpdate()
    {
        InputManager.SetCanInput(false);

        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);
        Vector3 angle = mainLight.gameObject.transform.localEulerAngles;

        mainLight.gameObject.transform.DOLocalRotate(new Vector3(defalutAngleX + 360, 0, 0), lightUpdateDuration, RotateMode.FastBeyond360).OnComplete(() =>
        {
            InputManager.SetCanInput(true);
            day = day.AddDays(1);

            Building.InitStaticCalcValue();
            Tile.income = 0;
            CalculateIncome();
            UpdateHappiness();
            EventManager.instance.EffectUpdate();

            ApplyDept();

            UIManager.instance.UpdateDailyInfo();

            lightCoroutine = StartCoroutine(DailyLight());
        }
        );
    }

    IEnumerator DailyLight()
    {
        while(mainLight.gameObject.transform.localEulerAngles.x > 5)
        {
            mainLight.gameObject.transform.Rotate(Vector3.right * Time.fixedDeltaTime * lightDailySpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnOffDailyLight(bool isOn)
    {
        if (isOn)
            lightCoroutine = StartCoroutine(DailyLight());
        else if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);
    }

    private void ApplyDept()
    {
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
    }

    private void CalculateDept()
    {
        if (!isPay)
        {
            SetCreditRating(1);
            InputManager.SetCanInput(false);
            if (creditRating >= maxCreditRating)
            {
                UIManager.instance.notifyObserver(EventState.GameOver);
            }
            else
            {
                weekResult = ResultType.Worst;
                debt = debtsOfWeek[GetWeekOfYear()] + Mathf.Max(totalIncome * 4, 0);
                UIManager.instance.notifyObserver(EventState.CreditScore);
            }
            return;
        }

        debt = debtsOfWeek[GetWeekOfYear()];
        isPay = false;

        UIManager.instance.SetDebtInfo();
        UIManager.instance.OnClickDeptDocButton();
    }

    public int GetWeekOfYear()
    {
        int weekOfYear = ((day.DayOfYear - 1) / 7) + (creditRating * -1) + ((day.Year - 2024) * 52);
        return weekOfYear;
    }

    private void SetCreditRating(int value)
    {
        creditRating += value;
        UIManager.instance.SetCreditScorePanel();
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
        totalIncome = total;
        ShopManager.instance.GetMoney(total);
        EventManager.instance.EventCostUpdate(total);
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
        AkSoundEngine.SetRTPCValue("HAPPY", cityHappiness);
    }

    public void PayDept()
    {
        DayOfWeek dayOfWeek = day.DayOfWeek;
        int dayOfWeekToInt = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        int money = ShopManager.instance.Money;

        if (isPay) return;
        if (!ShopManager.instance.PayMoney(debt)) return;

        AkSoundEngine.PostEvent("Play_Pay_001", gameObject);

        UIManager.instance.notifyObserver(EventState.Receipt);
        debt = 0;
        UIManager.instance.SetDebt();
        UIManager.instance.SetDebtInfo();

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
