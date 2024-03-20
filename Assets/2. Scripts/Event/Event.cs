using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum EventType { Problem, Event }
public enum ConditionType { Option, Influence, Exist }

[Serializable]
public class Event : MonoBehaviour
{
    public string eventName;
    public int eventIndex;
    public EventType type;
    public ConditionType conditionType;
    public ValueType valueType;
    public int targetIndex;
    public int duplication;
    public int curDay;
    public List<int> effectValue;
    public List<int> effectJackpotValue;

    public bool CheckCondition(Building building)
    {
        switch (conditionType)
        {
            case ConditionType.Option:
                return CheckCondition((OptionType)targetIndex, (ResidentialBuilding)building);
            case ConditionType.Influence:
                return CheckCondition((BuildingSubType)targetIndex, building);
            case ConditionType.Exist:
                return CheckCondition((BuildingType)targetIndex, building);
        }

        return false;
    }

    private bool CheckCondition(OptionType type, ResidentialBuilding building)
    {
        return building.CheckFacility(type);
    }

    private bool CheckCondition(BuildingSubType subType, Building building)
    {
        if (subType == BuildingSubType.Police || subType == BuildingSubType.FireFighting || building.type == BuildingType.Residential)
            return building.CheckInfluence(subType);

        return false;
    }

    private bool CheckCondition(BuildingType type, Building building)
    {
        int subType = (int)type - System.Enum.GetValues(typeof(BuildingType)).Length;
        if (subType >= -1)
            return building.subType == (BuildingSubType)subType;
        else
            return building.type == type;
    }
}