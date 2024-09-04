using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBuildingUI : MonoBehaviour, IObserver
{
    public void Notify(EventState state)
    {
        if (state == EventState.DestroyBuilding)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
