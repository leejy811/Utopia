using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNotifyUI : MonoBehaviour
{
    [Header("Building Intro")]
    public BuildingIntroUI[] buildingIntros;

    [Header("Event List")]
    public int curIndex;

    public void SetValue(Building building)
    {
        ShopManager.instance.SetObjectColor(building.gameObject, Color.red);

        int idx = building.type == BuildingType.Residential ? 0 : 1;
        int eventidx = building.curEvents.Count - 1;
        idx = idx * 2 + eventidx;

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
        curIndex = (curIndex + sign) % EventManager.instance.eventBuildings.Count;

        SetValue(EventManager.instance.eventBuildings[curIndex]);
    }

    public void Init()
    {
        curIndex = 0;
        SetValue(EventManager.instance.eventBuildings[curIndex]);
    }

    private void OnDisable()
    {
        ShopManager.instance.SetObjectColor(EventManager.instance.eventBuildings[curIndex].gameObject, Color.white);
    }
}