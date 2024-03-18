using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType { Problem, Event }
public enum ConditionType { Option, Influence, Exist }

[Serializable]
public class Event : MonoBehaviour
{
    public string eventName;
    public int eventIndex;
    public EventType type;
    public ConditionType conditionType;
    public int targetIndex;
    public int duplication;
    public int DurationTime;

    public bool CheckCondition(Building building)
    {
        switch(conditionType)
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
        return building.CheckInfluence(subType);
    }

    private bool CheckCondition(BuildingType type, Building building)
    {
        return building.type == type;
    }
}