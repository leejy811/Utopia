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
    }

    public bool CheckFacility(OptionType type)
    {
        return existFacility[(int)type];
    }

    public void BuyFacility(OptionType type)
    {
        existFacility[(int)type] = true;
    }

    public override int CalculateIncome()
    {
        if(happinessRate < 40)
            residentCnt.cur -= (int)(residentCnt.max * 0.1f);

        int res = residentCnt.cur * happinessRate * (4 - grade);
        return happinessRate >= 80 ? ((int)(res * 1.5f)) : res;
    }

    public override int CheckBonus()
    {
        int res = 0;

        if (commercialCSAT.cur > commercialCSAT.max)
            res += 1;
        else if (cultureCSAT.cur > cultureCSAT.max)
            res += 2;

        return res;
    }

    public override void UpdateHappiness()
    {
        //commercialCSAT
        if (commercialCSAT.cur > commercialCSAT.max)
        {
            happinessRate -= 1;
            ShopManager.instance.money -= 5;
        }
        else if (commercialCSAT.cur < commercialCSAT.min)
            happinessRate -= 2;
        else
            happinessRate += 1;

        //cultureCSAT
        if (cultureCSAT.cur > cultureCSAT.max)
        {
            happinessRate -= 1;
            ShopManager.instance.money -= 10;
        }
        else if (cultureCSAT.cur < cultureCSAT.min)
            happinessRate -= 3;
        else
            happinessRate += 1;

        //serviceCSAT
        if (serviceCSAT.cur > serviceCSAT.max)
            happinessRate -= 2;
        else if (serviceCSAT.cur < serviceCSAT.min)
            happinessRate -= 2;
        else
            happinessRate += 1;
    }
}
