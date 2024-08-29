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
    public float bonusRatio;

    public bool isPay;

    public Light mainLight;
    public float lightUpdateDuration;
    public float lightDailySpeed;
    public float defalutAngleX;

    public float playTime;
    private Coroutine lightCoroutine;
    private int totalIncome;
    private int payFailTime;

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
        playTime = 0.0f;
    }

    private void FixedUpdate()
    {
        playTime += Time.fixedDeltaTime;
    }

    public void DailyUpdate()
    {
        InputManager.SetCanInput(false);

        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);
        Vector3 angle = mainLight.gameObject.transform.localEulerAngles;

        Building.InitStaticCalcValue();

        Tile.income = 0;
        CalculateIncome();

        mainLight.gameObject.transform.DOLocalRotate(new Vector3(defalutAngleX + 360, 0, 0), lightUpdateDuration, RotateMode.FastBeyond360).OnComplete(() =>
        {
            InputManager.SetCanInput(true);
            day = day.AddDays(1);

            UpdateHappiness();
            EventManager.instance.EffectUpdate();
            EventManager.instance.CheckEvents();
            EventManager.instance.RandomRoulette(1);
            UIManager.instance.SetEventInfo(EventManager.instance.curEvents.ToArray());
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
        }

        int week = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
    }

    private void CalculateDept()
    {
        if (!isPay)
        {
            SetCreditRating(-25);
            payFailTime++;
            InputManager.SetCanInput(false);
            if (creditRating <= 0)
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
        int weekOfYear = ((day.DayOfYear - 1) / 7) + (payFailTime * -1) + ((day.Year - 2024) * 52);
        return weekOfYear;
    }

    private void SetCreditRating(int value)
    {
        creditRating = Mathf.Min(creditRating + value, 100);
        UIManager.instance.SetCreditScorePanel();
    }

    private void CalculateIncome()
    {
        int total = 0;
        int curMoney = ShopManager.instance.Money;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            total += building.CalculateIncome() + (int)(building.CalculateBonus() * day.DayOfYear * bonusRatio);
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

        UIManager.instance.DailyMoneyUpdate(curMoney, curMoney + total, lightUpdateDuration);
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
        RewardToPay();

        debt = 0;
        isPay = true;
        UIManager.instance.SetDebt();
        UIManager.instance.SetDebtInfo();

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
    }

    private void RewardToPay()
    {
        if (creditRating >= 100)
        {
            ShopManager.instance.GetMoney((int)(debt * 0.2f));
            UIManager.instance.notifyObserver(EventState.Payback);
        }
        else
        {
            UIManager.instance.notifyObserver(EventState.HealCredit);
            SetCreditRating(10);
        }
    }
}
