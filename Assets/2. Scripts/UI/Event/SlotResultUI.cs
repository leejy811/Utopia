using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotResultUI : MonoBehaviour
{
    public Image eventIconImage;
    public TextMeshProUGUI eventNameText;
    public TextMeshProUGUI eventDescriptText;

    public void SetValue(Event result, float second)
    {
        eventIconImage.sprite = result.eventIcon;
        eventNameText.text = result.eventName;
        eventDescriptText.text = result.eventEffectComment;

        transform.DOLocalMoveX(0, 1f);
    }

    public void ResetPosition(float second)
    {
        transform.DOLocalMoveX(-500, second);
    }

    private void OnDisable()
    {
        transform.localPosition = new Vector3(-500, transform.localPosition.y, transform.localPosition.z);
    }
}
