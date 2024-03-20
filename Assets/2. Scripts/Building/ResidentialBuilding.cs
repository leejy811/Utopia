using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum OptionType { Water = 0, Electricity, Sewage, SoundInsulation }

public class ResidentialBuilding : Building
{
    public bool[] existFacility;

    public BoundaryValue residentCnt;
    public BoundaryValue commercialCSAT;
    public BoundaryValue cultureCSAT;
    public BoundaryValue serviceCSAT;

    private void Awake()
    {
        existFacility = new bool[4];

        values[ValueType.Resident] = residentCnt;
        values[ValueType.CommercialCSAT] = commercialCSAT;
        values[ValueType.CultureCSAT] = cultureCSAT;
        values[ValueType.ServiceCSAT] = serviceCSAT;
    }

    public bool CheckFacility(OptionType type)
    {
        return existFacility[(int)type];
    }

    public void BuyFacility(OptionType type)
    {
        existFacility[(int)type] = true;
    }

    public void ApplyInfluence(BuildingType type, int value, bool isAdd)
    {
        BoundaryValue cast = values[(ValueType)type];
        cast.cur += isAdd ? value : -value;
        values[(ValueType)type] = cast;
    }

    public override int CalculateIncome()
    {
        if (happinessRate < 40)
        {
            BoundaryValue resident = values[ValueType.Resident];
            resident.cur -= (int)(values[ValueType.Resident].max * 0.1f);
            values[ValueType.Resident] = resident;
        }

        int res = values[ValueType.Resident].cur * happinessRate * (4 - grade);
        return happinessRate >= 80 ? ((int)(res * 1.5f)) : res;
    }

    public override int CheckBonus()
    {
        int res = 0;

        if (values[ValueType.CommercialCSAT].cur > values[ValueType.CommercialCSAT].max)
            res += 1;
        if (values[ValueType.CultureCSAT].cur > values[ValueType.CultureCSAT].max)
            res += 2;

        return res;
    }

    public override void UpdateHappiness()
    {
        //commercialCSAT
        if (values[ValueType.CommercialCSAT].cur > values[ValueType.CommercialCSAT].max)
        {
            happinessRate -= 1;
            ShopManager.instance.money -= 5;
        }
        else if (values[ValueType.CommercialCSAT].cur < values[ValueType.CommercialCSAT].min)
            happinessRate -= 2;
        else
            happinessRate += 1;

        //cultureCSAT
        if (values[ValueType.CultureCSAT].cur > values[ValueType.CultureCSAT].max)
        {
            happinessRate -= 1;
            ShopManager.instance.money -= 10;
        }
        else if (values[ValueType.CultureCSAT].cur < values[ValueType.CultureCSAT].min)
            happinessRate -= 3;
        else
            happinessRate += 1;

        //serviceCSAT
        if (values[ValueType.ServiceCSAT].cur > values[ValueType.ServiceCSAT].max)
            happinessRate -= 2;
        else if (values[ValueType.ServiceCSAT].cur < values[ValueType.ServiceCSAT].min)
            happinessRate -= 2;
        else
            happinessRate += 1;
    }
}
