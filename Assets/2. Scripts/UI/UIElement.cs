using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour, IObserver
{
    public EventState state;

    public void Notify(EventState state)
    {
        if(this.state == state && !gameObject.activeSelf)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
