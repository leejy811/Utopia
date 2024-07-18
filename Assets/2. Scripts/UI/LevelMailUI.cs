using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMailUI : MonoBehaviour
{
    public Button closeButton;
    public Transform[] mails;
    public float mailMoveSecond;

    private int curMailIdx;

    public void NextMail(bool isRight)
    {
        int sign = isRight ? 1 : -1;

        curMailIdx = Mathf.Clamp(curMailIdx + sign, 0, mails.Length - 1);

        mails[curMailIdx].SetAsLastSibling();

        if(curMailIdx == mails.Length - 1) 
            closeButton.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 500.0f, transform.localPosition.z);
        transform.DOLocalMoveY(0f, mailMoveSecond);
    }
}
