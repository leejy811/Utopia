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

    public bool interactable;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable)
            transform.DOScale(enterScale, tweenTime);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
            transform.DOScale(exitScale, tweenTime);
    }
}
