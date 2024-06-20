using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottomBlackUIAnimation : MonoBehaviour
{
    public float moveDistance = -400f;  
    public float duration = 3f;

    void Start()
    {
        StartCoroutine(DelayedAction());

    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(3f);

        RectTransform rectTransform = GetComponent<RectTransform>();


        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + moveDistance, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            InputManager.canInput = true;
            UIManager.instance.notifyObserver(EventState.CityLevelUp);
            UIManager.instance.SetCityLevelUp();
        });
    }
}