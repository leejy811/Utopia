using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventManager : MonoBehaviour
{
    static public EventManager instance;

    public List<List<Building>> targetBuildings = new List<List<Building>>();
    public List<Event> events;
    public List<Event> possibleEvents;
    public List<Event> curEvents;

    public bool isRoll;

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
        }
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
                    buildings[events[i].eventIndex].Add(building);
                    if (!possibleEvents.Contains(events[i]))
                        possibleEvents.Add(events[i]);
                }
            }
        }

        targetBuildings = buildings;
    }

    public void RandomRoulette()
    {
        if(possibleEvents.Count == 0) return;
        if (isRoll) return;

        isRoll = true;
        List<Event> ranEvents = new List<Event>();

        for(int i = 0;i < 3; i++)
        {
            int ranidx = Random.Range(0, possibleEvents.Count);
            ranEvents.Add(possibleEvents[ranidx]);
        }

        if (ranEvents[0] == ranEvents[1] && ranEvents[1] == ranEvents[2])
        {
            ranEvents[0].duplication = 3;
            ranEvents.RemoveAt(2);
            ranEvents.RemoveAt(1);
        }
        else if(ranEvents[0] == ranEvents[1])
        {
            ranEvents[0].duplication = 2;
            ranEvents[2].duplication = 1;
            ranEvents.RemoveAt(1);
        }
        else if(ranEvents[0] == ranEvents[2])
        {
            ranEvents[0].duplication = 2;
            ranEvents[1].duplication = 1;
            ranEvents.RemoveAt(2);
        }
        else if(ranEvents[1] == ranEvents[2])
        {
            ranEvents[1].duplication = 2;
            ranEvents[0].duplication = 1;
            ranEvents.RemoveAt(2);
        }
        else
        {
            ranEvents[0].duplication = 1;
            ranEvents[1].duplication = 1;
            ranEvents[2].duplication = 1;
        }

        ApplyEvents(ranEvents);
    }

    public void ApplyEvents(List<Event> ranEvents)
    {
        foreach(Event ranEvent in ranEvents)
        {
            if (!curEvents.Contains(ranEvent))
            {
                curEvents.Add(ranEvent);
                foreach (Building building in targetBuildings[ranEvent.eventIndex])
                {
                    if (ranEvent.type == EventType.Event)
                    {
                        if (ranEvent.duplication == 3)
                            building.ApplyEventEffect(ranEvent.effectJackpotValue[ranEvent.curDay], ranEvent.valueType);
                        else
                            building.ApplyEventEffect(ranEvent.effectValue[ranEvent.curDay], ranEvent.valueType);
                    }
                    building.ApplyEvent(ranEvent);
                }
            }
        }
    }

    public void EffectUpdate()
    {
        List<int> removeIdx = new List<int>();

        for (int i = 0; i < curEvents.Count; i++)
        {
            Event curevent = curEvents[i];
            curevent.curDay++;

            foreach (Building building in targetBuildings[curevent.eventIndex])
            {
                if (curevent.curDay >= curevent.effectValue.Count)
                {
                    building.RemoveEvent(curevent);
                    continue;
                }
                if (curevent.type == EventType.Event) continue;

                if (curevent.duplication == 3)
                    building.happinessRate -= curevent.effectJackpotValue[curevent.curDay - 1];
                else
                    building.happinessRate -= curevent.effectValue[curevent.curDay - 1] * curevent.duplication;
            }

            if (curevent.curDay >= curevent.effectValue.Count)
            {
                if (curevent.type == EventType.Event)
                {
                    foreach (Building building in targetBuildings[curevent.eventIndex])
                    {
                        if (curevent.duplication == 3)
                            building.ApplyEventEffect(curevent.effectJackpotValue[curevent.curDay - 1] * -1, curevent.valueType);
                        else
                            building.ApplyEventEffect(curevent.effectValue[curevent.curDay - 1] * -1, curevent.valueType);
                    }
                }
                removeIdx.Add(i);
            }
            else
                curEvents[i] = curevent;
        }

        for (int i = 0;i < removeIdx.Count;i++)
        {
            curEvents.RemoveAt(removeIdx[i] - i);
        }
    }
}
