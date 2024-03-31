using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum BuildingType { Residential = 0, Commercial, Culture, Service }
public enum BuildingSubType { Apartment = 0, Store, Cinema, Police, Restaurant, Gallery, Fire, Park }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }
public enum ValueType { None = 0, CommercialCSAT, CultureCSAT, ServiceCSAT, Happiness, Resident, user, utility, Influence }
public enum BoundaryType { Less = -1, Include, More }

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

    public BoundaryType CheckBoundary()
    {
        if (cur > max)
            return BoundaryType.More;
        else if (cur < min)
            return BoundaryType.Less;
        else
            return BoundaryType.Include;
    }
}

public class Building : MonoBehaviour
{
    public int count;
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
        ApplyInfluenceToTile(influencePower);
    }

    private void OnDestroy()
    {
        ApplyInfluenceToTile(-influencePower);
    }

    public void ChangeViewState(ViewStateType state)
    {
        viewState = state;

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, (float)state / 2.0f);
            }
        }
    }

    protected void ApplyInfluenceToTile(int power)
    {
        Vector3 size = new Vector3(influenceSize * 2 - 1, 1, influenceSize * 2 - 1);

        Collider[] hits = Physics.OverlapBox(transform.position, size * 0.5f, transform.rotation, LayerMask.GetMask("Tile"));

        foreach (Collider hit in hits)
        {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            if(tile.building != gameObject)
            {
                tile.SetInfluenceValue(type, subType, power);
            }
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
                ApplyEventProblem(curevent, false);
            }

            curEvents[i] = curevent;
        }

        for (int i = 0; i < removeIdx.Count; i++)
        {
            curEvents.RemoveAt(removeIdx[i] - i);
        }
    }

    public void SetHappiness(int amount)
    {
        if (happinessRate + amount < 0)
        {
            happinessRate = 0;
            return;
        }

        happinessRate += amount;
    }

    public void SolveEvent(int index)
    {
        if (curEvents[index / 2].type == EventType.Event) return;
        int ran = Random.Range(0, 100);

        if (ran < curEvents[index / 2].solutions[index % 2].prob)
        {
            if(curEvents[index / 2].conditionType == ConditionType.Option)
            {
                (this as ResidentialBuilding).BuyFacility((OptionType)curEvents[index / 2].targetIndex);
            }

            SetSolveEvent(index / 2);
        }
    }

    protected void SetSolveEvent(int index)
    {
        RoutineManager.instance.SetCityHappiness(happinessRate, 0);
        ApplyEventProblem(curEvents[index], true);
        curEvents.RemoveAt(index);
    }

    public List<Event> GetEventProblem()
    {
        List<Event> events = new List<Event>();
        foreach (Event curEvent in curEvents)
        {
            if (curEvent.type == EventType.Problem)
                events.Add(curEvent);
        }
        return events;
    }

    public void SetPosition(Vector3 position)
    {
        this.position = new Vector2Int((int)position.x, (int)position.z);
    }

    public bool CheckInfluence(BuildingSubType type)
    {
        return Grid.instance.tiles[position.x, position.y].subInfluenceValues[(int)type] != 0;
    }

    public void SolveEventToInfluence(BuildingSubType type)
    {
        for(int i = 0;i < curEvents.Count; i++)
        {
            if (curEvents[i].conditionType == ConditionType.Influence && curEvents[i].targetIndex == (int)type)
            {
                SetSolveEvent(i);
                break;
            }
        }
    }

    public void ApplyEventEffect(int amount, ValueType type)
    {
        if(type == ValueType.Influence)
        {
            ApplyInfluenceToTile((int)(influencePower * (amount / 100.0f)));
            return;
        }

        BoundaryValue value = values[type];
        value.cur *= (int)(1.0f  + amount / 100.0f);

        if((value.CheckBoundary() == BoundaryType.More && values[type].CheckBoundary() == BoundaryType.Include)
            || (value.CheckBoundary() == BoundaryType.Include && values[type].CheckBoundary() == BoundaryType.Less))
            ApplyInfluenceToTile((int)(influencePower * 0.5f));
        else if ((value.CheckBoundary() == BoundaryType.Include && values[type].CheckBoundary() == BoundaryType.More)
            || (value.CheckBoundary() == BoundaryType.Less && values[type].CheckBoundary() == BoundaryType.Include))
            ApplyInfluenceToTile((int)(influencePower * -0.5f));
        else if(value.CheckBoundary() == BoundaryType.More && values[type].CheckBoundary() == BoundaryType.Less)
            ApplyInfluenceToTile(influencePower);
        else if (value.CheckBoundary() == BoundaryType.Less && values[type].CheckBoundary() == BoundaryType.More)
            ApplyInfluenceToTile(-influencePower);
        values[type] = value;
    }
    
    public void ApplyEventProblem(Event curevent, bool isAdd)
    {
        int sign = isAdd ? 1 : -1;
        if (curevent.duplication == 3)
            SetHappiness(curevent.effectJackpotValue[curevent.curDay - 1] * sign);
        else
            SetHappiness(curevent.effectValue[curevent.curDay - 1] * curevent.duplication * sign);
    }

    public virtual int CalculateIncome()
    {
        return 0;
    }
    public virtual int CalculateBonus()
    {
        return 0;
    }

    public virtual void UpdateHappiness()
    {
        
    }

    public virtual void ApplyInfluence(int value, BuildingType type)
    {
        if (!values.ContainsKey((ValueType)type)) return;

        BoundaryValue cast = values[(ValueType)type];
        cast.cur += value;
        values[(ValueType)type] = cast;
    }
}
