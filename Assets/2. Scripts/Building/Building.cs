using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum BuildingType { Residential = 0, Commercial, Culture, Service }
public enum BuildingSubType { Apartment = 0, Store, Cinema, Police, Restaurant, Gallery, Fire, Park, Hospital, Entertainment }
public enum ViewStateType { Transparent = 0, Translucent, Opaque }
public enum ValueType { None = 0, CommercialCSAT, CultureCSAT, ServiceCSAT, Happiness, Resident, user, utility, Influence, Tax, Cost, FinalIncome }
public enum BoundaryType { Less = -1, Include, More }

[Serializable]
public struct BoundaryValue 
{ 
    public float max, min, cur; 
    
    public BoundaryValue(float max, float min, float cur)
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
    public string buildingName;
    public int index;
    public int count;
    public int grade;
    public BuildingType type;
    public BuildingSubType subType;
    public ViewStateType viewState;
    public int happinessRate;
    public int happinessDifference;

    public Vector2Int position;
    public int influencePower;
    public int additionalInfluencePower;
    public int influenceSize;
    public GameObject influenceObject;

    public List<Event> curEvents;
    public int cost;

    public Sprite buildingIcon;

    public Dictionary<ValueType, BoundaryValue> values = new Dictionary<ValueType, BoundaryValue>();

    private void Start()
    {
        int power = type == BuildingType.Residential ? (int)values[ValueType.Resident].cur : influencePower;
        ApplyInfluenceToTile(power, true, true);
    }

    public virtual void DestroyBuilding()
    {
        int power = type == BuildingType.Residential ? (int)values[ValueType.Resident].cur : influencePower;

        foreach (Event curEvent in curEvents)
        {
            if (curEvent.valueType == ValueType.Influence)
            {
                power += (int)(power * (curEvent.effectValue[0] / 100.0f));
            }
        }

        ApplyInfluenceToTile(-power, true, false);
    }

    public void ChangeViewState(ViewStateType state)
    {
        viewState = state;

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, ((float)state / 2.0f) + 0.1f);
            }
        }
    }

    protected void ApplyInfluenceToTile(int power, bool isInit, bool subEnable = false)
    {
        Vector3 size = new Vector3(influenceSize * 2 - 1, 1, influenceSize * 2 - 1);

        Collider[] hits = Physics.OverlapBox(transform.position, size * 0.5f, transform.rotation, LayerMask.GetMask("Tile"));

        foreach (Collider hit in hits)
        {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            if(tile.transform.position != gameObject.transform.position)
            {
                tile.SetInfluenceValue(type, power);

                if (isInit)
                    tile.SetSubInfluenceValue(subType, subEnable);
            }
        }
    }

    public void ApplyEvent(Event newEvent)
    {
        if (!CheckApplyEvent(newEvent)) return;

        if (newEvent.type == EventType.Event)
            ApplyEventEffect(newEvent.effectValue[newEvent.curDay], newEvent.valueType, true);
        else if (newEvent.type == EventType.Problem)
            ApplyEventProblem(newEvent, false);

        if (GetEventProblemCount() == 0 && newEvent.type == EventType.Problem)
            EventManager.instance.SetEventBuildings(this, true);
        curEvents.Add(newEvent);
    }

    public void LoadEvent(Event newEvent)
    {
        if (newEvent.type == EventType.Event)
            ApplyEventEffect(newEvent.effectValue[newEvent.curDay], newEvent.valueType, true);

        if (GetEventProblemCount() == 0 && newEvent.type == EventType.Problem)
            EventManager.instance.SetEventBuildings(this, true);
        curEvents.Add(newEvent);
    }

    public bool CheckApplyEvent(Event newEvent)
    {
        return !curEvents.Contains(newEvent) && curEvents.Count < 2;
    }

    public void UpdateEventEffect()
    {
        List<int> removeIdx = new List<int>();

        for (int i = 0; i < curEvents.Count; i++)
        {
            Event curevent = curEvents[i];
            curevent.curDay++;

            if (curevent.curDay >= curevent.effectValue.Count)
            {
                if (curevent.type == EventType.Event)
                {
                    ApplyEventEffect(curevent.effectValue[curevent.curDay - 1], curevent.valueType, false);
                }
                removeIdx.Add(i);
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
            amount = happinessRate * -1;
        else if (happinessRate + amount > 100)
            amount = 100 - happinessRate;

        happinessDifference += amount;
        happinessRate += amount;

        if (type != BuildingType.Residential)
        {
            if (happinessRate == 0 && happinessDifference != 0)
            {
                ApplyInfluenceToTile(-(influencePower + additionalInfluencePower), false);
            }
            else if (happinessRate == happinessDifference && happinessRate != 0)
            {
                ApplyInfluenceToTile(influencePower + additionalInfluencePower, false);
            }
        }
    }

    public void SolveEvent(int index)
    {
        int ran = Random.Range(0, 100);

        if (ran < curEvents[index].solutions[0].prob)
        {
            if (curEvents[index].conditionType == ConditionType.Option && curEvents[index].solutions[0].prob == 100)
            {
                (this as ResidentialBuilding).BuyFacility((OptionType)curEvents[index].targetIndex);
            }
            else
                SetSolveEvent(index);
        }
    }

    protected void SetSolveEvent(int index)
    {
        ApplyEventProblem(curEvents[index], true);
        if (GetEventProblemCount() == 1 && curEvents[index].type == EventType.Problem)
            EventManager.instance.SetEventBuildings(this, false);
        curEvents.Remove(curEvents[index]);
        AkSoundEngine.PostEvent("Play_UI_solve_001", gameObject);
    }

    public int GetEventProblemCount()
    {
        int cnt = 0;

        foreach (Event curEvent in curEvents)
        {
            if (curEvent.type == EventType.Problem)
                cnt++;
        }
        return cnt;
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

    public void ApplyEventEffect(int amount, ValueType type, bool enable)
    {
        if(type == ValueType.Influence)
        {
            if (!enable) amount *= -1;
            if (happinessRate != 0)
                ApplyInfluenceToTile((int)(influencePower * (amount / 100.0f)), false);
            additionalInfluencePower += (int)(influencePower * (amount / 100.0f));
            return;
        }

        BoundaryValue value = values[type];
        float percent = enable ? ((100.0f + amount) / 100.0f) : (100.0f / (100.0f + amount));
        value.cur = value.cur * percent;
        values[type] = value;
    }

    public void ApplyEventProblem(Event curevent, bool isAdd)
    {
        int sign = isAdd ? 1 : -1;
        int value = curevent.effectValue[curevent.curDay] * sign;
        SetHappiness(value);
        RoutineManager.instance.SetCityHappiness(value, 0);
    }

    public void SetActiveInfluenceObject(bool active)
    {
        influenceObject.transform.localScale = new Vector3(influenceSize * 2 + 1, 1, influenceSize * 2 + 1);
        influenceObject.SetActive(active);
    }

    public virtual int CalculateIncome()
    {
        return 0;
    }
    public virtual int CalculateBonus(bool isExpect = false, int valueType = 0)
    {
        return 0;
    }

    public virtual int UpdateHappiness(bool isExpect = false, int valueType = 0)
    {
        return 0;
    }

    public virtual void ApplyInfluence(int value, BuildingType type, bool isInit = false)
    {
       
    }

    public void ExpectHappiness(BuildingType type)
    {
        int amount = UpdateHappiness(true, (int)type);
        UIManager.instance.SetHappinessPopUp(amount, transform.position);
    }

    public static void InitStaticCalcValue()
    {
        ResidentialBuilding.income = 0;
        ResidentialBuilding.bonusCost = 0;
        ResidentialBuilding.bonusIncome = 0;

        CommercialBuilding.income = 0;
        CommercialBuilding.bonusCost = 0;
        CommercialBuilding.bonusIncome = 0;

        CultureBuilding.income = 0;
        CultureBuilding.bonusCost = 0;
        CultureBuilding.bonusIncome = 0;

        ServiceBuilding.income = 0;
        ServiceBuilding.bonusCost = 0;
        ServiceBuilding.bonusIncome = 0;
    }

    protected float GetIncomeEvent()
    {
        float total = 0.0f;

        foreach (Event globalEvent in EventManager.instance.globalEvents)
        {
            if (globalEvent.valueType == ValueType.Tax && globalEvent.targetIndex == (int)type)
            {
                total += globalEvent.effectValue[0] / 100.0f;
            }
        }

        return total;
    }
}
