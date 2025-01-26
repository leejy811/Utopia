using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EventSolutionPanel
{
    public GameObject panel;
    public TextMeshProUGUI eventSolText;
    public TextMeshProUGUI eventProbText;
    public TextMeshProUGUI eventDayText;
    public TextMeshProUGUI eventCostText;

    public void SetValue(Event curEvent)
    {
        eventSolText.text = curEvent.solutions[0].name;
        eventProbText.text = curEvent.solutions[0].prob.ToString() + "%";
        eventDayText.text = "D-" + (curEvent.effectValue.Count - curEvent.curDay).ToString();
        eventCostText.text = "-" + string.Format("{0:#,###}", curEvent.solutions[0].cost) + "¿ø";
    }
}

public class EventNotifyUI : MonoBehaviour, IObserver
{
    [Header("UIElement")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI listText;
    public Image[] eventIconImages;
    public EventMouseOverUI[] eventMouseOvers;
    public EventSolutionPanel[] eventSolutionPanels;

    [Header("Event List")]
    public int curIndex;

    [Header("Camera")]
    public CameraPriorityController cameraPriorityController;

    private bool[] isClicked = new bool[2];

    public void SetValue(Building building)
    {
        ShopManager.instance.SetObjectColor(building.gameObject, Color.red);

        nameText.text = building.buildingName;
        happinessText.text = building.happinessRate + (building.happinessDifference  < 0 ? " (-" : " (+") + Mathf.Abs(building.happinessDifference) + ")%";
        listText.text = (curIndex + 1).ToString() + "/" + EventManager.instance.eventBuildings.Count.ToString();
        for(int i = 0;i < eventIconImages.Length; i++)
        {
            if (building.curEvents.Count > i)
            {
                eventMouseOvers[i].interactable = true;
                eventMouseOvers[i].GetComponent<Button>().interactable = true;
                eventIconImages[i].sprite = building.curEvents[i].eventIcon;
                eventSolutionPanels[i].SetValue(building.curEvents[i]);
            }
            else
            {
                eventMouseOvers[i].interactable = false;
                eventMouseOvers[i].GetComponent<Button>().interactable = false;
                eventIconImages[i].enabled = false;
                eventSolutionPanels[i].panel.SetActive(false);
            }
        }
    }

    public void NextBuilding(bool isRight)
    {
        if (curIndex < EventManager.instance.eventBuildings.Count)
            ShopManager.instance.SetObjectColor(EventManager.instance.eventBuildings[curIndex].gameObject, Color.white);

        int sign = isRight ? 1 : -1;
        curIndex = (curIndex + sign + EventManager.instance.eventBuildings.Count) % EventManager.instance.eventBuildings.Count;

        SetValue(EventManager.instance.eventBuildings[curIndex]);
        cameraPriorityController.ChangeLookTarget(EventManager.instance.eventBuildings[curIndex].transform);
    }

    public void Init()
    {
        curIndex = 0;
        isClicked[0] = false;
        isClicked[1] = false;

        if (EventManager.instance.eventBuildings.Count != 0)
        {
            SetValue(EventManager.instance.eventBuildings[curIndex]);
            cameraPriorityController.ChangeActiveState();
            cameraPriorityController.ChangeLookTarget(EventManager.instance.eventBuildings[curIndex].transform);
        }
    }

    public void OnClickEventButton(int index)
    {
        List<Event> events = EventManager.instance.eventBuildings[curIndex].curEvents;
        isClicked[index] = !isClicked[index];
        if (!isClicked[index])
        {
            for (int i = 0; i < eventMouseOvers.Length; i++)
            {
                if (events.Count > i)
                    eventMouseOvers[i].interactable = true;
            }
            eventSolutionPanels[index].panel.SetActive(false);
        }
        else
        {
            eventSolutionPanels[index].panel.SetActive(true);
            eventSolutionPanels[(index + 1) % 2].panel.SetActive(false);

            eventSolutionPanels[index].SetValue(events[index]);

            eventMouseOvers[index].interactable = false;
            eventMouseOvers[(index + 1) % 2].interactable = false;

            isClicked[(index + 1) % 2] = false;
        }
    }

    public void OnDisable()
    {
        if (EventManager.instance.eventBuildings.Count != 0 && curIndex < EventManager.instance.eventBuildings.Count && EventManager.instance.eventBuildings[curIndex].gameObject != null)
            ShopManager.instance.SetObjectColor(EventManager.instance.eventBuildings[curIndex].gameObject, Color.white);

        cameraPriorityController.ChangeActiveState();

        for(int i = 0;i < eventSolutionPanels.Length; i++)
        {
            eventSolutionPanels[i].panel.SetActive(false);
        }
    }

    public void Notify(EventState state)
    {
        if(state == EventState.EventNotify && !gameObject.activeSelf)
        {
            ShopManager.instance.ChangeState(BuyState.EventCheck);
            gameObject.SetActive(true);
            Init();
            AkSoundEngine.SetRTPCValue("BOOL", 2);
        }
        else
        {
            AkSoundEngine.SetRTPCValue("BOOL", 1);
            gameObject.SetActive(false);
        }
    }
}