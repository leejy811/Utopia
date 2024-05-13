using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventState { None, EventNotify, CityLevel, CityLevelUp, Statistic, 
                        Construct, EtcFunc, EventIcon, TileInfluence, TileColor }

public interface IObserver
{
    void Notify(EventState state);
}


public interface ISubject
{
    void addObserver(IObserver observer);
    void removeObserver(IObserver observer);
    void notifyObserver(EventState state);
}