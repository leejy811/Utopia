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

        List<Event> ranEvents = new List<Event>();

        for(int i = 0;i < 3; i++)
        {
            int ranidx = Random.Range(0, possibleEvents.Count);
            ranEvents.Add(possibleEvents[ranidx]);
        }

        UIManager.instance.SetRoulette(ranEvents);

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
            List<int> indexs = new List<int>();
            int range = Mathf.RoundToInt((targetBuildings[ranEvent.eventIndex].Count / 2.0f) + 0.1f);

            for (int i = 0;i < range; i++)
            {
                int ranIdx = Random.Range(0, range);
                while (indexs.Contains(ranIdx))
                {
                    ranIdx = Random.Range(0, range);
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
    }
}
