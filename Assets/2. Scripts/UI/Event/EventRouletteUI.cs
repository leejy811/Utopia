using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventRouletteUI : MonoBehaviour
{
    public void SetValue(List<Event> events)
    {
        gameObject.GetComponent<SlotMachineManager>().SetEvent(events);
    }
}
