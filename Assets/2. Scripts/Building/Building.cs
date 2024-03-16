using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Residential = -1, Commrcial, culture, Service }
public enum BuildingSubType { Apartment, Store, Movie, Police, Restaurant, Art, FireFighting, Park }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }

[Serializable]
public struct BoundaryValue { public int max, min, cur; }

public class Building : MonoBehaviour
{
    [SerializeField, Range(0, 3)] protected int grade;
    [SerializeField] protected BuildingType type;
    [SerializeField] protected BuildingSubType subType;
    [SerializeField] protected ViewStateType viewState;
    [SerializeField, Range(0, 100)] protected int happinessRate;
    [SerializeField] protected int influencePower;
    [SerializeField] protected int influenceSize;

    public int cost;

    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        ApplyInfluenceToTile(true);
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

    private void OnDestroy()
    {
        ApplyInfluenceToTile(false);
    }

    public void ChangeViewState(ViewStateType state)
    {
        viewState = state;
        material.color = new Color(material.color.r, material.color.g, material.color.b, (float)state / 2.0f);
    }

    protected void ApplyInfluenceToTile(bool isAdd)
    {
        if (type == BuildingType.Residential) return;

        Vector3 size = new Vector3(influenceSize * 2 - 1, 1, influenceSize * 2 - 1);

        Collider[] hits = Physics.OverlapBox(transform.position, size * 0.5f, transform.rotation, LayerMask.GetMask("Tile"));

        foreach (Collider hit in hits)
        {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            tile.influenceValues[(int)type] += isAdd ? influencePower : -influencePower;
        }
    }
}
