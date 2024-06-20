using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image[] images;

    private void Start()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].transform.localPosition += Vector3.up * -100 * i;
        }
    }

    public void SetSlot(Event[] events)
    {
        for (int i = 0; i < images.Length - 1; i++)
        {
            images[i].sprite = events[i].eventIcon;
        }
        images[images.Length - 1].sprite = events[0].eventIcon;
    }
}
