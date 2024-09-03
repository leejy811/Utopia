using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventState { None, EventNotify, CityLevelUp, SlotMachine,
                        Construct, EtcFunc, EventIcon, TileColor, LockButton, DebtDoc,
                        PaySuccess, HealCredit, LateReceipt, CityLevel, PayFail, GameOver, 
                        Menu, Setting, GameClear, Tutorial, DestroyBuilding, Phone,
                        GameStart, ConstructButton, ConstructBuilding, SocialEffect }

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