using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventMouseOverUI : UIMouseOver
{
    public GameObject panel;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;

        base.OnPointerEnter(eventData);
        panel.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;
        panel.SetActive(false);
    }
}
