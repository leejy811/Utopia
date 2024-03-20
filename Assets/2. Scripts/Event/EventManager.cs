using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventManager : MonoBehaviour
{
    static public EventManager instance;

    public Dictionary<Event, List<Building>> targetBuildings = new Dictionary<Event, List<Building>>();
    public List<Event> events;
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
        foreach(Event e in events)
        {
            targetBuildings[e] = new List<Building>();
        }
    }

    public void CheckEvents()
    {
        targetBuildings.Clear();
        curEvents.Clear();

        foreach (Event e in events)
        {
            foreach (Building building in BuildingSpawner.instance.buildings)
            {
                if (e.CheckCondition(building))
                {
                    targetBuildings[e].Add(building);
                    if (!curEvents.Contains(e))
                        curEvents.Add(e);
                }
            }
        }
    }

    public void RandomRoulette()
    {
        List<Event> ranEvents = new List<Event>();

        for(int i = 0;i < 3; i++)
        {
            int ranidx = Random.Range(0, curEvents.Count);
            ranEvents.Add(curEvents[ranidx]);
        }

        if (ranEvents[0] == ranEvents[1] && ranEvents[1] == ranEvents[2])
        {
            ranEvents[0].duplication = 3;
            ranEvents.RemoveAt(1);
            ranEvents.RemoveAt(2);
        }
        else if(ranEvents[0] == ranEvents[1])
        {
            ranEvents[0].duplication = 2;
            ranEvents.RemoveAt(1);
        }
        else if(ranEvents[0] == ranEvents[2])
        {
            ranEvents[0].duplication = 2;
            ranEvents.RemoveAt(2);
        }
        else if(ranEvents[1] == ranEvents[2])
        {
            ranEvents[0].duplication = 2;
            ranEvents.RemoveAt(2);
        }

        ApplyEvents(ranEvents);
    }

    public void ApplyEvents(List<Event> ranEvents)
    {
        foreach(Event ranEvent in ranEvents)
        {
            foreach(Building building in targetBuildings[ranEvent])
            {
                if (curEvents[i].duplication == 3)
                    building.ApplyEventEffect(ranEvent.effectJackpotValue[ranEvent.curDay - 1], ranEvent.valueType);
                else
                    building.ApplyEventEffect(ranEvent.effectValue[ranEvent.curDay - 1], ranEvent.valueType);
                building.ApplyEvent(ranEvent);
            }
        }
    }

    public void EffectUpdate()
    {
        for (int i = 0; i < curEvents.Count; i++)
        {
            curEvents[i].curDay++;

            foreach (Building building in targetBuildings[curEvents[i]])
            {
                if (curEvents[i].type == EventType.Event) break;

                if (curEvents[i].duplication == 3)
                    building.happinessRate -= curEvents[i].effectJackpotValue[curEvents[i].curDay - 1];
                else
                    building.happinessRate -= curEvents[i].effectValue[curEvents[i].curDay - 1] * curEvents[i].duplication;
            }

            if (curEvents[i].curDay == curEvents[i].effectValue.Count)
            {
                if (curEvents[i].type == EventType.Event)
                {
                    foreach (Building building in targetBuildings[curEvents[i]])
                    {
                        if (curEvents[i].duplication == 3)
                            building.ApplyEventEffect(curEvents[i].effectJackpotValue[curEvents[i].curDay - 1] * -1, curEvents[i].valueType);
                        else
                            building.ApplyEventEffect(curEvents[i].effectValue[curEvents[i].curDay - 1] * -1, curEvents[i].valueType);
                    }
                    curEvents.RemoveAt(i);
                }
            }
        }
    }
}
