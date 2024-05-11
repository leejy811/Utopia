using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageTextAnimation : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    public float duration = 1.5f;  
    public float delay = 2.0f;    

    void Start()
    {
        Vector3 initialScale = image.rectTransform.localScale;
        initialScale.y = 0.5f;  
        image.rectTransform.localScale = initialScale;
        text.text = "";  
    }

    public void TriggerAnimation()
    {

        // 이미지 세로 확대 애니메이션
        Vector3 targetScale = image.rectTransform.localScale;
        targetScale.y = 1.0f;  // 원래 세로 크기로 확대
        image.rectTransform.DOScale(targetScale, duration).OnComplete(() =>
        {
            // 일정 시간 대기 후, 텍스트와 이미지 세로 축소
            DOVirtual.DelayedCall(delay, () =>
            {
                text.text = "";  // 텍스트 클리어
                Vector3 shrinkScale = image.rectTransform.localScale;
                shrinkScale.y = 0.5f;  // 세로 크기를 다시 50%로 축소
                image.rectTransform.DOScale(shrinkScale, duration);
            });
        });
    }
}
