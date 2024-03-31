using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum OptionType { Water = 0, Electricity, Sewage, SoundInsulation }

public class ResidentialBuilding : Building
{
    public static int cityResident;

    public bool[] existFacility;

    public BoundaryValue residentCnt;
    public BoundaryValue commercialCSAT;
    public BoundaryValue cultureCSAT;
    public BoundaryValue serviceCSAT;

    private void Awake()
    {
        existFacility = new bool[System.Enum.GetValues(typeof(OptionType)).Length];

        if(grade <= 2)
        {
            existFacility[(int)OptionType.Water] = true;
            existFacility[(int)OptionType.Electricity] = true;
            if(grade == 1)
            {
                existFacility[(int)OptionType.Sewage] = true;
                existFacility[(int)OptionType.SoundInsulation] = true;
            }
        }

        values[ValueType.Resident] = residentCnt;
        values[ValueType.CommercialCSAT] = commercialCSAT;
        values[ValueType.CultureCSAT] = cultureCSAT;
        values[ValueType.ServiceCSAT] = serviceCSAT;

        influencePower = residentCnt.cur;
        cityResident += residentCnt.cur;
    }

    private void OnDestroy()
    {
        cityResident -= residentCnt.cur;
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
            BoundaryValue resident = values[ValueType.Resident];
            resident.cur -= (int)(values[ValueType.Resident].max * 0.1f);
            cityResident -= (int)(values[ValueType.Resident].max * 0.1f);
            values[ValueType.Resident] = resident;
        }

        float res = values[ValueType.Resident].cur * (happinessRate / 100.0f) * (4 - grade);
        return happinessRate >= 80 ? ((int)(res * 1.5f)) : (int)res;
    }

    public override int CalculateBonus()
    {
        int res = 0;

        if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.More)
            res -= 50;
        if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.More)
            res -= 100;

        return res;
    }

    public override void UpdateHappiness()
    {
        //commercialCSAT
        if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.More)
        {
            SetHappiness(-1);
        }
        else if (values[ValueType.CommercialCSAT].CheckBoundary() == BoundaryType.Less)
            SetHappiness(-2);
        else
            SetHappiness(1);

        //cultureCSAT
        if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.More)
        {
            SetHappiness(-1);
        }
        else if (values[ValueType.CultureCSAT].CheckBoundary() == BoundaryType.Less)
            SetHappiness(-3);
        else
            SetHappiness(1);

        //serviceCSAT
        if (values[ValueType.ServiceCSAT].CheckBoundary() == BoundaryType.More)
            SetHappiness(-2);
        else if (values[ValueType.ServiceCSAT].CheckBoundary() == BoundaryType.Less)
            SetHappiness(-2);
        else
            SetHappiness(1);
    }
}
