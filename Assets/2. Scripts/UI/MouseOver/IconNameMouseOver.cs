using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconNameMouseOver : UIMouseOver
{
    public GameObject iconName;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        iconName.gameObject.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        iconName.gameObject.SetActive(false);
    }
}
