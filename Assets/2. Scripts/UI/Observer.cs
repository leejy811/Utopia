using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventState { None, EventNotify, CityLevelUp, SlotMachine,
                        Construct, EtcFunc, EventIcon, TileColor, LockButton, DebtDoc,
                        Payback, HealCredit, LateReceipt, CityLevel, CreditScore, GameOver, 
                        Menu, Setting, GameClear, Tutorial, DestroyBuilding }

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