using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                building.ApplyEvent(ranEvent);
            }
        }
    }
}
