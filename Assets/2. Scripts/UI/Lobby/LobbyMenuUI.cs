using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuUI : MonoBehaviour, IObserver
{
    public void Notify(EventState state)
    {
        if (state == EventState.None)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
