using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameUI : MonoBehaviour, IObserver
{
    protected EnterBuilding curGameBuilding;

    public virtual void InitGame(EnterBuilding building)
    {
        gameObject.SetActive(true);
        curGameBuilding = building;
    }

    public virtual void SetValue()
    {

    }

    public void Notify(EventState state)
    {
        if (state != EventState.Minigame)
            gameObject.SetActive(false);
    }
}
