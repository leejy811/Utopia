using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class CultureBilding : MonoBehaviour
{
    [SerializeField] private int influencePower;

    [SerializeField] private BoundaryValue trendValue;
    [SerializeField] private BoundaryValue fee;
}
