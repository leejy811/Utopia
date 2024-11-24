using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    static public EventManager instance;

    [Header("Count")]
    public int curCountDay;
    public int preCountDay;
    public Vector2Int randomCount;

    [Header("Event List")]
    public List<List<Building>> targetBuildings = new List<List<Building>>();
    public List<Building> eventBuildings;
    public List<Event> events;
    public List<Event> possibleEvents;
    public List<Event> globalEvents;
    public List<Event> curEvents;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < events.Count; i++)
        {
            targetBuildings.Add(new List<Building>());

            Event e = events[i];
            e.eventIndex = i;
            events[i] = e;
        }

        ResetCount();
    }
    
    public bool CheckEventCondition()
    {
        if (possibleEvents.Count == 0)
            return false;
        else if (CityLevelManager.instance.levelIdx == 0)
            return false;
        else if (curEvents.Count > 3)
            return false;
        else
            return true;
    }

    public void CheckEvents()
    {
        List<List<Building>> buildings = new List<List<Building>>();

        for (int i = 0; i < events.Count; i++)
        {
            buildings.Add(new List<Building>());
        }

        possibleEvents.Clear();

        for (int i = 0; i < events.Count; i++)
        {
            bool isPossible = true;
            for (int j = 0; j < curEvents.Count; j++)
            {
                if (events[i].type == EventType.Problem)
                {
                    if (events[i].eventIndex == curEvents[j].eventIndex)
                    {
                        isPossible = false;
                        break;
                    }
                }
                else if (events[i].eventIndex == curEvents[j].eventIndex || events[i].targetIndex == curEvents[j].targetIndex)
                {
                    isPossible = false;
                    break;
                }
            }
            if (!isPossible) continue;

            foreach (Building building in BuildingSpawner.instance.buildings)
            {
                if (events[i].CheckCondition(building))
                {
                    if (building.CheckApplyEvent(events[i]))
                    {
                        buildings[events[i].eventIndex].Add(building);
                        if (!possibleEvents.Contains(events[i]))
                            possibleEvents.Add(events[i]);
                    }
                }
            }
        }

        targetBuildings = buildings;
    }

    private void ResetCount()
    {
        curCountDay = 0;
        preCountDay = Random.Range(randomCount.x, randomCount.y);
    }

    public void RandomRoulette(int eventNum)
    {
        if (!CheckEventCondition()) return;

        curCountDay++;
        if (curCountDay != preCountDay) return;
        ResetCount();

        Event ranEvent = possibleEvents[Random.Range(0, possibleEvents.Count)];

        ApplyEvents(ranEvent);
    }

    public void ApplyEvents(Event ranEvent)
    {
        curEvents.Add(ranEvent);
        UIManager.instance.SetEventTempPopUp(ranEvent, transform.position);

        if (ranEvent.type == EventType.Global)
        {
            globalEvents.Add(ranEvent);
            return;
        }

        List<int> indexs = new List<int>();

        for (int i = 0; i < targetBuildings[ranEvent.eventIndex].Count; i++)
        {
            int ranIdx = Random.Range(0, targetBuildings[ranEvent.eventIndex].Count);
            while (indexs.Contains(ranIdx))
            {
                ranIdx = Random.Range(0, targetBuildings[ranEvent.eventIndex].Count);
            }
            indexs.Add(ranIdx);
            targetBuildings[ranEvent.eventIndex][ranIdx].ApplyEvent(ranEvent);
        }

    }

    public void EffectUpdate()
    {
        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            building.UpdateEventEffect();
        }

        List<int> removeIdx = new List<int>();
        for (int i = 0; i < eventBuildings.Count; i++)
        {
            if (eventBuildings[i].GetEventProblemCount() == 0)
                removeIdx.Add(i);
        }

        for (int i = 0; i < removeIdx.Count; i++)
        {
            eventBuildings.RemoveAt(removeIdx[i] - i);
        }

        globalEvents = EventDayUpdate(globalEvents);
        curEvents = EventDayUpdate(curEvents);
    }

    private List<Event> EventDayUpdate(List<Event> events)
    {
        List<int> removeIdx = new List<int>();

        for (int i = 0; i < events.Count; i++)
        {
            Event e = events[i];
            e.curDay++;
            events[i] = e;

            if(e.curDay >= e.effectValue.Count)
            {
                removeIdx.Add(i);
            }
        }

        for (int i = 0; i < removeIdx.Count; i++)
        {
            events.RemoveAt(removeIdx[i] - i);
        }

        return events;
    }

    public void SetEventBuildings(Building building, bool isAdd)
    {
        if (isAdd)
            eventBuildings.Add(building);
        else
        {
            for(int i = 0;i < eventBuildings.Count; i++)
            {
                if (ReferenceEquals(eventBuildings[i], building))
                {
                    ShopManager.instance.SetObjectColor(eventBuildings[i].gameObject, Color.white);
                    
                    if (UIManager.instance.eventNotify.gameObject.activeSelf)
                    {
                        if (eventBuildings.Count == 1)
                            UIManager.instance.notifyObserver(EventState.None);
                        else if (i == 0)
                        {
                            UIManager.instance.OnClickEventNotifyNext(true);
                            UIManager.instance.eventNotify.curIndex--;
                        }
                        else
                            UIManager.instance.OnClickEventNotifyNext(false);
                    }

                    eventBuildings.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public float GetFinalIncomeEventValue()
    {
        float res = 1.0f;
        foreach(Event globalEvent in globalEvents)
        {
            if(globalEvent.valueType == ValueType.FinalIncome)
                res += globalEvent.effectValue[0] / 100.0f;
        }
        return res;
    }

    public float GetBuildCostEventValue()
    {
        float res = 1.0f;
        foreach (Event globalEvent in globalEvents)
        {
            if (globalEvent.valueType == ValueType.Cost && globalEvent.targetIndex == (int)BuyState.BuyBuilding)
                res += globalEvent.effectValue[0] / 100.0f;
        }
        return res;
    }
}
