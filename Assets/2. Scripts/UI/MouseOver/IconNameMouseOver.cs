using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconNameMouseOver : UIMouseOver, IObserver
{
    public GameObject iconName;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        iconName.gameObject.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        iconName.gameObject.SetActive(false);
    }

    public void Notify(EventState state)
    {
        if (state != EventState.TileColor)
            iconName.gameObject.SetActive(false);
    }
}
