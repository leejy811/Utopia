using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineBrain;

public class EventManager : MonoBehaviour
{
    static public EventManager instance;

    [Header("Count")]
    public int curCountDay;
    public int preCountDay;
    public Vector2Int randomCount;

    [Header("Value")]
    public int[] eventCount;
    [Range(0.0f, 1.0f)] public float eventApplyRatio;

    [Header("Event List")]
    public List<List<Building>> targetBuildings = new List<List<Building>>();
    public List<Building> eventBuildings;
    public List<Event> events;
    public List<Event> possibleEvents;
    public List<Event> globalEvents;
    public List<Event> curEvents;

    public int cost { get; private set; }

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

        //ToDo : 토토피아 추가되면 SelectRewardType 함수 조건 달아서 추가

        //UIManager.instance.eventRoulette.SetPossibleEvent(possibleEvents.ToArray());

        targetBuildings = buildings;
        globalEvents.Clear();
    }

    private void SelectRewardType()
    {
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

        for (int i = 0; i < rewardTypeEvent.Count; i++)
        {
            List<int> indexs = new List<int>();
            int range = rewardTypeEvent[i].Count - eventCount[i];

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
    }

    public bool PayRoulleteCost()
    {
        if (!ShopManager.instance.PayMoney(cost)) return false;
        else return true;
    }

    public void EventCostUpdate(int totalIncome)
    {
        if (totalIncome > 0)
        {
            totalIncome = Mathf.FloorToInt(totalIncome / 400.0f);
            cost = Mathf.Max(totalIncome * 100, 100);
        }
        else
        {
            int money = Mathf.FloorToInt(ShopManager.instance.Money / 800.0f);
            cost = Mathf.Max(money * 100, 100);
        }
    }

    private void ResetCount()
    {
        curCountDay = 0;
        preCountDay = Random.Range(randomCount.x, randomCount.y);
    }

    private List<Event> RemoveDuplicate(List<Event> ranEvents)
    {
        List<Event> temp = new List<Event>();
        int idx = 0;

        for (int i = 0; i < ranEvents.Count; i++)
        {
            Event e = ranEvents[i];
            e.duplication = 1;
            ranEvents[i] = e;
        }

        for (int i = 0;i < ranEvents.Count; i++)
        {
            if (temp.Contains(ranEvents[idx]))
            {
                int tmpIdx = temp.FindIndex(x => x == ranEvents[idx]);
                Event e = temp[tmpIdx];
                e.duplication++;
                temp[tmpIdx] = e;
                ranEvents.RemoveAt(idx);
            }
            else
            {
                temp.Add(ranEvents[idx]);
                idx++;
            }
        }

        if (temp.Count == 1 && temp[0].duplication == 3)
            ResetCount();

        return temp;
    }

    public void RandomRoulette(int eventNum)
    {
        if (!CheckEventCondition()) return;

        List<Event> ranEvents = new List<Event>();
        List<int> ranIdx = new List<int>();
        curCountDay++;

        for (int i = 0; i < eventNum; i++)
        {
            ranIdx.Add(Random.Range(0, possibleEvents.Count));
            ranEvents.Add(possibleEvents[ranIdx[i]]);
        }

        if (curCountDay == preCountDay)
        {
            //if (토토피아 모드면)
            //{
            //    ranIdx.Add(Random.Range(0, possibleEvents.Count));
            //    ranIdx.Add(ranIdx[0]);
            //    ranIdx.Add(ranIdx[0]);
            //    ranEvents.Add(possibleEvents[ranIdx[0]]);
            //    ranEvents.Add(possibleEvents[ranIdx[0]]);
            //    ranEvents.Add(possibleEvents[ranIdx[0]]);
            //}

            ResetCount();
        }
        else
        {
            //if (일반모드면)
            return;
        }

        //if (UIManager.instance.eventRoulette != null)
        //    UIManager.instance.SetRoulettePopUp(ranIdx.ToArray());

        ranEvents = RemoveDuplicate(ranEvents);

        ApplyEvents(ranEvents);
    }

    public void ApplyEvents(List<Event> ranEvents)
    {
        foreach (Event ranEvent in ranEvents)
        {
            curEvents.Add(ranEvent);
            UIManager.instance.SetEventTempPopUp(ranEvent, transform.position);

            if (ranEvent.type == EventType.Global)
            {
                globalEvents.Add(ranEvent);
                continue;
            }

            List<int> indexs = new List<int>();
            int range = Mathf.CeilToInt(targetBuildings[ranEvent.eventIndex].Count * eventApplyRatio);

            for (int i = 0; i < range; i++)
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
