using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType { Problem, Event, Global }
public enum ConditionType { Option, Influence, Exist, None }

[Serializable]
public struct EventSolution 
{
    public string name;
    public int cost, prob; 
}

[Serializable]
public struct Event
{
    public string eventName;
    public string eventEffectComment;
    public Sprite eventIcon;
    public int eventIndex;
    public EventType type;
    public ConditionType conditionType;
    public ValueType valueType;
    public int targetIndex;
    public int duplication;
    public int curDay;
    public List<EventSolution> solutions;
    public List<int> effectValue;
    public List<int> effectJackpotValue;

    public bool CheckCondition(Building building)
    {
        switch (conditionType)
        {
            case ConditionType.Option:
                return CheckCondition((OptionType)targetIndex, building);
            case ConditionType.Influence:
                return CheckCondition((BuildingSubType)targetIndex, building);
            case ConditionType.Exist:
                return CheckCondition((BuildingType)targetIndex, building);
            case ConditionType.None:
                return true;
        }

        return false;
    }

    private bool CheckCondition(OptionType type, Building building)
    {
        if (building.type != BuildingType.Residential) return false;

        return !(building as ResidentialBuilding).CheckFacility(type);
    }

    private bool CheckCondition(BuildingSubType subType, Building building)
    {
        if (subType == BuildingSubType.Police || subType == BuildingSubType.Fire
            || subType == BuildingSubType.Park || subType == BuildingSubType.Hospital || building.type == BuildingType.Residential)
            return !building.CheckInfluence(subType);

        return false;
    }

    private bool CheckCondition(BuildingType type, Building building)
    {
        int subType = (int)type - System.Enum.GetValues(typeof(BuildingType)).Length;
        if (subType >= 0)
            return building.subType == (BuildingSubType)subType;
        else
            return building.type == type;
    }

    public override int GetHashCode()
    {
        return eventIndex.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Event e && e.eventIndex == this.eventIndex;
    }

    public static bool operator ==(Event left, Event right)
    {
        return left.eventIndex == right.eventIndex;
    }
    public static bool operator !=(Event left, Event right)
    {
        return left.eventIndex != right.eventIndex;
    }

    public int GetEffectValue(int day)
    {
        int res = duplication == 3 ? effectJackpotValue[day] : effectValue[day] * duplication;

        return res;
    }

    public string GetEventToString()
    {
        string res = "";
        string[] valueString = { "상품가격", "입장료", "취업률" };

        if (valueType == ValueType.utility)
            res += valueString[targetIndex - 1];
        else if (valueType == ValueType.Influence)
            res += "영향력";
        else
            res += "행복도";

        if (duplication == 3)
            res += " <color=#FF0000>대폭</color>";

        if (type == EventType.Problem)
            res += " 감소";
        else
            res += GetEffectValue(0) > 0 ? " 증가" : " 감소";

        return res;
    }
}