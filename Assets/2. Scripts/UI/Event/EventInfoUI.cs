using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventInfoUI : MonoBehaviour
{
    public Image[] eventPanels;
    public Image[] eventImages;
    public EventDetailUI[] eventDetails;

    public void SetValue(Event[] curEvents)
    {
        for (int i = 0;i < eventImages.Length;i++)
        {
            if (i < curEvents.Length)
            {
                eventPanels[i].gameObject.SetActive(true);
                eventImages[i].sprite = curEvents[i].eventIcon;
                eventDetails[i].eventDescriptionText.text = curEvents[i].eventEffectComment;
            }
            else
            {
                eventPanels[i].gameObject.SetActive(false);
            }
        }
    }
}