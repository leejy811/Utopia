using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType { Problem, Event}

[Serializable]
public class Event : MonoBehaviour
{
    public string eventName;
    public EventType type;
    public int DurationTime;
}