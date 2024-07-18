using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMailUI : MonoBehaviour
{
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;
    public Transform[] mails;
    public float mailMoveSecond;

    private int curMailIdx;

    public void NextMail(bool isRight)
    {
        int sign = isRight ? 1 : -1;

        curMailIdx = Mathf.Clamp(curMailIdx + sign, 0, mails.Length - 1);

        mails[curMailIdx].SetAsLastSibling();

        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);

        if (curMailIdx == mails.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
        }
        else if (curMailIdx == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 500.0f, transform.localPosition.z);
        transform.DOLocalMoveY(0f, mailMoveSecond);
        InitButton();
    }

    private void InitButton()
    {
        prevButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (mails.Length == 1 ) 
        {
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
        }
    }
}
