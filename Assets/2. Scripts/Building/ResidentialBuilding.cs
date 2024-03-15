using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OptionType { Water = 0, Electricity, Sewage, SoundInsulation }

[RequireComponent(typeof(Building))]
public class ResidentialBuilding : MonoBehaviour
{
    [SerializeField] private bool[] existFacility;

    [SerializeField] private BoundaryValue residentCnt;
    [SerializeField] private BoundaryValue commercialCSAT;
    [SerializeField] private BoundaryValue cultureCSAT;
    [SerializeField] private BoundaryValue serviceCSAT;
}
