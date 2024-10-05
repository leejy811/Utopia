using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TopBlackUIAnimation : MonoBehaviour
{
    public float moveDistance = 400f;  
    public float duration = 3f;  

    void Start()
    {
        StartCoroutine(DelayedAction());

    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.5f);

        RectTransform rectTransform = GetComponent<RectTransform>();


        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + moveDistance, duration).SetEase(Ease.InOutQuad);
    }
}