using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum OptionType { Water = 0, Sewage, Electricity, SoundInsulation }

public class ResidentialBuilding : Building
{
    public static int cityResident;
    public static int yesterDayResident;
    public static int income;
    public static int bonusCost;

    public bool[] existFacility;

    public BoundaryValue residentCnt;
    public BoundaryValue commercialCSAT;
    public BoundaryValue cultureCSAT;
    public BoundaryValue serviceCSAT;

    private void Awake()
    {
        values[ValueType.Resident] = residentCnt;
        values[ValueType.CommercialCSAT] = commercialCSAT;
        values[ValueType.CultureCSAT] = cultureCSAT;
        values[ValueType.ServiceCSAT] = serviceCSAT;

        influencePower = residentCnt.cur;
        cityResident += residentCnt.cur;
        UIManager.instance.SetCityResident();
        CityLevelManager.instance.UpdateCityType();
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        cityResident -= residentCnt.cur;
        UIManager.instance.SetCityResident();
    }

    public bool CheckFacility(OptionType type)
    {
        return existFacility[(int)type];
    }

    public void BuyFacility(OptionType type)
    {
        existFacility[(int)type] = true;
        SolveEventToOption(type);
    }

    private void SolveEventToOption(OptionType type)
    {
        for (int i = 0; i < curEvents.Count; i++)
        {
            if (curEvents[i].conditionType == ConditionType.Option && curEvents[i].targetIndex == (int)type)
            {
                SetSolveEvent(i);
                break;
            }
        }
    }

    public override int CalculateIncome()
    {
        if (happinessRate < 40)
        {
            int value = (int)(values[ValueType.Resident].max * -0.1f);

            if (values[ValueType.Resident].cur + value < 0)
                value = -values[ValueType.Resident].cur;

            BoundaryValue resident = values[ValueType.Resident];
            resident.cur += value;
            cityResident += value;
            values[ValueType.Resident] = resident;

            ApplyInfluenceToTile(value, false);
        }

        float res = values[ValueType.Resident].cur * (happinessRate / 100.0f) * (4 - grade);
        res += res * GetIncomeEvent();
        res = happinessRate >= 80 ? ((int)(res * 1.5f)) : (int)res;
        income += (int)res;
        return (int)res;
    }

    public override int CalculateBonus()
    {
        int res = 0;

        if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.More)
            res -= 100;
        if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.More)
            res -= 50;

        bonusCost += res;
        return res;
    }

    public override int UpdateHappiness(bool isExpect)
    {
        int changeAmount = 0;
        //commercialCSAT
        if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.More)
            changeAmount += 0;
        else if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.Less)
            changeAmount += -2;
        else
            changeAmount += 1;

        //cultureCSAT
        if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.More)
            changeAmount += 0;
        else if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.Less)
            changeAmount += -2;
        else
            changeAmount += 1;

        //serviceCSAT
        if (values[ValueType.ServiceCSAT].CheckBoundary() == BoundaryType.More)
            changeAmount += 0;
        else if (values[ValueType.ServiceCSAT].CheckBoundary() == BoundaryType.Less)
            changeAmount += -3;
        else
            changeAmount += 1;

        if(isExpect)
            return changeAmount;

        SetHappiness(changeAmount);
        return 0;
    }

    public override void ApplyInfluence(int value, BuildingType type)
    {
        if (!values.ContainsKey((ValueType)type)) return;

        BoundaryValue cast = values[(ValueType)type];
        cast.cur += value;
        values[(ValueType)type] = cast;

        int amount = UpdateHappiness(true);
        UIManager.instance.SetHappinessPopUp(amount, transform.position);
    }
}
