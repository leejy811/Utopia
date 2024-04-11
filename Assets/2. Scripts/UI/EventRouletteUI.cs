using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventRouletteUI : MonoBehaviour
{
    [Header("Image")]
    public Image[] eventImages;

    public void SetValue(List<Event> events)
    {
        for(int i = 0;i < eventImages.Length;i++)
        {
            eventImages[i].sprite = events[i].eventIcon;
        }
    }
}
