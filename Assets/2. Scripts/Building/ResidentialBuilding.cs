using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class ResidentialBuilding : MonoBehaviour
{
    [SerializeField] private bool existWaterFac;
    [SerializeField] private bool existElectricityFac;
    [SerializeField] private bool existSewageFac;
    [SerializeField] private bool existSoundInsulFac;

    [SerializeField] private BoundaryValue residentCnt;
    [SerializeField] private BoundaryValue commercialCSAT;
    [SerializeField] private BoundaryValue cultureCSAT;
    [SerializeField] private BoundaryValue serviceCSAT;
}
