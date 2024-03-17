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

    public void RandomRoulette()
    {
        curEvents.Clear();

        for(int i = 0;i < 3; i++)
        {
            int ranidx = Random.Range(0, events.Count);
            curEvents.Add(events[ranidx]);
        }

        ApplyEvents();
    }

    public void ApplyEvents()
    {
        foreach(Event ce in curEvents)
        {
            foreach(Building building in BuildingSpawner.instance.buildings)
            {
                building.ApplyEvent(ce);
            }
        }
    }
}
