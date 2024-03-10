using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class ServiceBuilding : MonoBehaviour
{
    [SerializeField] private int costPerDay;
    [SerializeField] private int influencePower;

    [SerializeField] private BoundaryValue employmentValue;
}
