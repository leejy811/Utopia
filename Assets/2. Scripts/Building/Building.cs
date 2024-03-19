using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Residential = -1, Commercial, culture, Service }
public enum BuildingSubType { Apartment = -1, Store, Movie, Police, Restaurant, Art, FireFighting, Park }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }

[Serializable]
public struct BoundaryValue { public int max, min, cur; }

public class Building : MonoBehaviour
{
    public int grade;
    public BuildingType type;
    public BuildingSubType subType;
    public ViewStateType viewState;
    public int happinessRate;

    public Vector2Int position;
    public int influencePower;
    public int influenceSize;

    public List<Event> curEvents;
    public int cost;

    private void Start()
    {
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

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers)
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, (float)state / 2.0f);
        }
    }

    protected void ApplyInfluenceToTile(bool isAdd)
    {
        if (type == BuildingType.Residential) return;

        Vector3 size = new Vector3(influenceSize * 2 - 1, 1, influenceSize * 2 - 1);

        Collider[] hits = Physics.OverlapBox(transform.position, size * 0.5f, transform.rotation, LayerMask.GetMask("Tile"));

        foreach (Collider hit in hits)
        {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            tile.SetInfluenceValue(type, subType, influencePower, true);
        }
    }

    public void ApplyEvent(Event newEvent)
    {
        if (!curEvents.Contains(newEvent))
        {
            curEvents.Add(newEvent);
        }
    }

    public void SetPosition(Vector3 position)
    {
        this.position = new Vector2Int((int)position.x, (int)position.z);
    }

    public bool CheckInfluence(BuildingSubType type)
    {
        return Grid.instance.tiles[position.x, position.y].subInfluenceValues[(int)type] != 0;
    }

    public virtual int CalculateIncome()
    {
        return 0;
    }

    public virtual int CheckBonus()
    {
        return 0;
    }

    public virtual void UpdateHappiness()
    {
        
    }
}
