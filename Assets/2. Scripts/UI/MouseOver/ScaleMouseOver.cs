using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleMouseOver : UIMouseOver
{
    public float tweenTime;
    public float enterScale;
    public float exitScale;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;

        base.OnPointerEnter(eventData);
        transform.DOScale(enterScale, tweenTime);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;

        transform.DOScale(exitScale, tweenTime);
    }
}
