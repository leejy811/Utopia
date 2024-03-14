using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Residential, Commrcial, culture, Service }
public enum BuildingSubType { Apartment, Store, Movie, Police, Restaurant, Art, FireFighting, Park }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }

[Serializable]
public struct BoundaryValue { public int max, min, cur; }

public class Building : MonoBehaviour
{
    [SerializeField, Range(0, 3)] private int grade;
    [SerializeField] private BuildingType type;
    [SerializeField] private BuildingSubType subType;
    [SerializeField, Range(0, 100)] private int happinessRate;
    [SerializeField] private ViewStateType viewState;

    public int cost;

    Material material;

    private void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
    }

    private void Update()
    {
        BuyState state = ShopManager.instance.buyState;
        switch (state)
        {
            case BuyState.None:
            case BuyState.SellBuilding:
                ChangeViewState(ViewStateType.Opaque);
                break;
            case BuyState.BuyBuilding:
                ChangeViewState(ViewStateType.Translucent);
                break;
            case BuyState.BuyTile:
            case BuyState.BuildTile:
                ChangeViewState(ViewStateType.Transparent);
                break;
        }
    }

    public void ChangeViewState(ViewStateType state)
    {
        viewState = state;
        material.color = new Color(1, 1, 1, (float)state / 2.0f);
    }
}
