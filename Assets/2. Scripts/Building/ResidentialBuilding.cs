using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OptionType { Water = 0, Electricity, Sewage, SoundInsulation }

public class ResidentialBuilding : Building
{
    [SerializeField] private bool[] existFacility;

    [SerializeField] private BoundaryValue residentCnt;
    [SerializeField] private BoundaryValue commercialCSAT;
    [SerializeField] private BoundaryValue cultureCSAT;
    [SerializeField] private BoundaryValue serviceCSAT;

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
