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
            images[i].sprite = EventManager.instance.events[i].eventIcon;
        }
        images[40].sprite = EventManager.instance.events[0].eventIcon;
    }
}
