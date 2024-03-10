using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class CommercialBuilding : MonoBehaviour
{
    [SerializeField] private int influencePower;

    [SerializeField] private BoundaryValue customerCnt;
    [SerializeField] private BoundaryValue productPrice;
}
