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

        // �̹��� ���� Ȯ�� �ִϸ��̼�
        Vector3 targetScale = image.rectTransform.localScale;
        targetScale.y = 1.0f;  // ���� ���� ũ��� Ȯ��
        image.rectTransform.DOScale(targetScale, duration).OnComplete(() =>
        {
            // ���� �ð� ��� ��, �ؽ�Ʈ�� �̹��� ���� ���
            DOVirtual.DelayedCall(delay, () =>
            {
                text.text = "";  // �ؽ�Ʈ Ŭ����
                Vector3 shrinkScale = image.rectTransform.localScale;
                shrinkScale.y = 0.5f;  // ���� ũ�⸦ �ٽ� 50%�� ���
                image.rectTransform.DOScale(shrinkScale, duration);
            });
        });
    }
}
