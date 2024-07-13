using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineBrain;

[System.Serializable]
public struct EventInfo
{
    public int[] eventCount;
    [Range(0.0f, 1.0f)] public float eventProb;
}

public class EventManager : MonoBehaviour
{
    static public EventManager instance;

    [Header("Jackpot")]
    public int curJackpotDay;
    public int jackpotDay;

    [Header("Value")]
    public int[] eventCondition;
    public EventInfo[] eventInfos;

    [Header("Event List")]
    public List<List<Building>> targetBuildings = new List<List<Building>>();
    public List<Event> events;
    public List<Event> possibleEvents;
    public List<Event> globalEvents;
    public List<Building> eventBuildings;

    [Header("Cost")]
    public int initialCost;
    public int costMultiplier;

    private int rollTimes;
    private int cost;

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

        curJackpotDay = 0;
        jackpotDay = Random.Range(7, 15);
    }
    
    public bool CheckEventCondition()
    {
        if (BuildingSpawner.instance.buildingTypeCount[0] < eventCondition[0]
            || BuildingSpawner.instance.buildingTypeCount[1] < eventCondition[1]
            || BuildingSpawner.instance.buildingTypeCount[2] < eventCondition[2]
            || BuildingSpawner.instance.buildingTypeCount[3] < eventCondition[3])
        {
            return false;
        }
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

        List<List<Event>> rewardTypeEvent = new List<List<Event>>
        {
            new List<Event>(),
            new List<Event>(),
            new List<Event>()
        };

        for (int i = 0; i < possibleEvents.Count; i++)
        {
            rewardTypeEvent[(int)possibleEvents[i].rewardType].Add(possibleEvents[i]);
        }

        for (int i = 0;i < rewardTypeEvent.Count; i++)
        {
            List<int> indexs = new List<int>();
            int range = rewardTypeEvent[i].Count - eventInfos[(int)RoutineManager.instance.weekResult].eventCount[i];

            for (int j = 0; j < range; j++)
            {
                int ranIdx = Random.Range(0, rewardTypeEvent[i].Count);
                while (indexs.Contains(ranIdx))
                {
                    ranIdx = Random.Range(0, rewardTypeEvent[i].Count);
                }
                indexs.Add(ranIdx);
                possibleEvents.Remove(rewardTypeEvent[i][ranIdx]);
            }
        }

        targetBuildings = buildings;
        globalEvents.Clear();
    }

    public bool PayRoulleteCost()
    {
        if (rollTimes <= 3)
            cost = initialCost;
        else
            cost = costMultiplier * rollTimes + initialCost;

        if (!ShopManager.instance.PayMoney(cost)) return false;
        else return true;
    }

    public void RandomRoulette()
    {
        if(possibleEvents.Count == 0) return;

        List<Event> ranEvents = new List<Event>();
        List<int> ranIdx = new List<int>();
        curJackpotDay++;

        if (curJackpotDay == jackpotDay)
        {
            ranIdx.Add(Random.Range(0, possibleEvents.Count));
            ranIdx.Add(ranIdx[0]);
            ranIdx.Add(ranIdx[0]);
            ranEvents.Add(possibleEvents[ranIdx[0]]);
            ranEvents.Add(possibleEvents[ranIdx[0]]);
            ranEvents.Add(possibleEvents[ranIdx[0]]);

            curJackpotDay = 0;
            jackpotDay = Random.Range(7, 15);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                ranIdx.Add(Random.Range(0, possibleEvents.Count));
                ranEvents.Add(possibleEvents[ranIdx[i]]);
            }
        }

        UIManager.instance.SetRoulettePopUp(ranIdx.ToArray());

        Event e;
        if (ranEvents[0] == ranEvents[1] && ranEvents[1] == ranEvents[2])
        {
            e = ranEvents[0];
            e.duplication = 3;
            ranEvents[0] = e;
            ranEvents.RemoveAt(2);
            ranEvents.RemoveAt(1);
        }
        else if(ranEvents[0] == ranEvents[1])
        {
            e = ranEvents[0];
            e.duplication = 2;
            ranEvents[0] = e;

            e = ranEvents[2];
            e.duplication = 1;
            ranEvents[2] = e;

            ranEvents.RemoveAt(1);
        }
        else if(ranEvents[0] == ranEvents[2])
        {
            e = ranEvents[0];
            e.duplication = 2;
            ranEvents[0] = e;

            e = ranEvents[1];
            e.duplication = 1;
            ranEvents[1] = e;
            ranEvents.RemoveAt(2);
        }
        else if(ranEvents[1] == ranEvents[2])
        {
            e = ranEvents[1];
            e.duplication = 2;
            ranEvents[1] = e;

            e = ranEvents[0];
            e.duplication = 1;
            ranEvents[0] = e;
            ranEvents.RemoveAt(2);
        }
        else
        {
            e = ranEvents[0];
            e.duplication = 1;
            ranEvents[0] = e;

            e = ranEvents[1];
            e.duplication = 1;
            ranEvents[1] = e;

            e = ranEvents[2];
            e.duplication = 1;
            ranEvents[2] = e;
        }

        ApplyEvents(ranEvents);
    }

    public void ApplyEvents(List<Event> ranEvents)
    {
        foreach (Event ranEvent in ranEvents)
        {
            if (ranEvent.type == EventType.Global)
            {
                globalEvents.Add(ranEvent);
                continue;
            }
            
            List<int> indexs = new List<int>();
            int range = Mathf.CeilToInt((targetBuildings[ranEvent.eventIndex].Count / 3.0f));

            for (int i = 0;i < range; i++)
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
    }

    public void CostUpdate()
    {
        cost = initialCost;
        rollTimes = 0;
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
                            UIManager.instance.notifyObserver(EventState.EventNotify);
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
                res += globalEvent.GetEffectValue(0) / 100.0f;
        }
        return res;
    }

    public float GetBuildCostEventValue()
    {
        float res = 1.0f;
        foreach (Event globalEvent in globalEvents)
        {
            if (globalEvent.valueType == ValueType.Cost && globalEvent.targetIndex == (int)BuyState.BuyBuilding)
                res += globalEvent.GetEffectValue(0) / 100.0f;
        }
        return res;
    }
}
