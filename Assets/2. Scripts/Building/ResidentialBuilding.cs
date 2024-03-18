using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
