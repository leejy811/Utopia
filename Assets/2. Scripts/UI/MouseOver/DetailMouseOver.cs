using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetailMouseOver : UIMouseOver
{
    public DetailUI detail;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        detail.gameObject.SetActive(true);
        detail.SetValue();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        detail.gameObject.SetActive(false);
    }
}
