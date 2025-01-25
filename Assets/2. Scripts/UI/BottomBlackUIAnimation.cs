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
        yield return new WaitForSeconds(0.5f);

        RectTransform rectTransform = GetComponent<RectTransform>();

        if (GameManager.instance.isLoad)
            DataBaseManager.instance.Load();

        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + moveDistance, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            UIManager.instance.MovePanelAnim(2f, false);

            if (GameManager.instance.skipTutorial)
                InputManager.SetCanInput(true);
            else
                UIManager.instance.SetTutorialPopup();
        });
    }
}