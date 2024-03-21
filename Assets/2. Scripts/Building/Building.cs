using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BuildingType { Residential = -1, Commercial, culture, Service }
public enum BuildingSubType { Apartment = -1, Store, Movie, Police, Restaurant, Art, FireFighting, Park }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }
public enum ValueType { None = -1, CommercialCSAT, CultureCSAT, ServiceCSAT, Happiness, Resident, Customer, Product, Trend, Fee, Employment }

[Serializable]
public struct BoundaryValue 
{ 
    public int max, min, cur; 
    
    public BoundaryValue(int max, int min, int cur)
    {
        this.max = max;
        this.min = min;
        this.cur = cur;
    }
}

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

    public Dictionary<ValueType, BoundaryValue> values = new Dictionary<ValueType, BoundaryValue>();

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
        if (!curEvents.Contains(newEvent) && curEvents.Count < 2)
        {
            if (newEvent.type == EventType.Event)
            {
                if (newEvent.duplication == 3)
                    ApplyEventEffect(newEvent.effectJackpotValue[newEvent.curDay], newEvent.valueType);
                else
                    ApplyEventEffect(newEvent.effectValue[newEvent.curDay], newEvent.valueType);
            }
            curEvents.Add(newEvent);
        }
    }

    public void UpdateEventEffect()
    {
        List<int> removeIdx = new List<int>();

        for (int i = 0; i < curEvents.Count; i++)
        {
            Event curevent = curEvents[i];
            curevent.curDay++;

            if(curevent.curDay >= curevent.effectValue.Count)
            {
                removeIdx.Add(i);
                if(curevent.type == EventType.Event)
                {
                    if (curevent.duplication == 3)
                        ApplyEventEffect(curevent.effectJackpotValue[curevent.curDay - 1] * -1, curevent.valueType);
                    else
                        ApplyEventEffect(curevent.effectValue[curevent.curDay - 1] * -1, curevent.valueType);
                }
                continue;
            }

            if (curevent.type == EventType.Problem)
            {
                if (curevent.duplication == 3)
                    happinessRate -= curevent.effectJackpotValue[curevent.curDay - 1];
                else
                    happinessRate -= curevent.effectValue[curevent.curDay - 1] * curevent.duplication;
            }

            curEvents[i] = curevent;
        }

        for (int i = 0; i < removeIdx.Count; i++)
        {
            curEvents.RemoveAt(removeIdx[i] - i);
        }
    }

    public void SolveEvent(int index)
    {
        if (curEvents[index / 2].type == EventType.Event) return;
        int ran = Random.Range(0, 100);

        if (ran < curEvents[index / 2].solutions[index % 2].prob)
            curEvents.RemoveAt(index / 2);
    }

    public void SetPosition(Vector3 position)
    {
        this.position = new Vector2Int((int)position.x, (int)position.z);
    }

    public bool CheckInfluence(BuildingSubType type)
    {
        return Grid.instance.tiles[position.x, position.y].subInfluenceValues[(int)type] != 0;
    }

    public void ApplyEventEffect(int amount, ValueType type)
    {
        BoundaryValue value = values[type];
        value.cur += amount;
        values[type] = value;
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
