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
    public List<Building> eventBuildings;

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

        UIManager.instance.SetRoulettePopUp(true, ranEvents);

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
                            UIManager.instance.SetEventNotifyPopUp(false);
                        else if(i == 0)
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
}
