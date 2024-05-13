using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNotifyUI : MonoBehaviour, IObserver
{
    [Header("Building Intro")]
    public BuildingIntroUI[] buildingIntros;

    [Header("Event List")]
    public int curIndex;

    [Header("Camera")]
    public CameraPriorityController cameraPriorityController;

    public void SetValue(Building building)
    {
        ShopManager.instance.SetObjectColor(building.gameObject, Color.red);

        int idx = building.type == BuildingType.Residential ? 0 : 1;

        for(int i = 0;i < buildingIntros.Length;i++)
        {
            if (idx == i)
                buildingIntros[i].gameObject.SetActive(true);
            else
                buildingIntros[i].gameObject.SetActive(false);
        }

        buildingIntros[idx].SetValue(building);
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

        if (EventManager.instance.eventBuildings.Count != 0)
        {
            SetValue(EventManager.instance.eventBuildings[curIndex]);
            cameraPriorityController.ChangeActiveState();
            cameraPriorityController.ChangeLookTarget(EventManager.instance.eventBuildings[curIndex].transform);
        }
    }

    public void OnDisable()
    {
        if (EventManager.instance.eventBuildings.Count != 0 && curIndex < EventManager.instance.eventBuildings.Count && EventManager.instance.eventBuildings[curIndex].gameObject != null)
            ShopManager.instance.SetObjectColor(EventManager.instance.eventBuildings[curIndex].gameObject, Color.white);

        cameraPriorityController.ChangeActiveState();
    }

    public void Notify(EventState state)
    {
        if(state == EventState.EventNotify && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Init();
        }
        else
            gameObject.SetActive(false);
    }
}