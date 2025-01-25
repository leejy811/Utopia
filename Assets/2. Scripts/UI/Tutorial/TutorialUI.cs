using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct TutorialClip
{
    public VideoClip videoClip;
    [TextArea] public string description;
}

public class TutorialUI : MonoBehaviour, IObserver
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Text")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI indexText;

    [Header("Button")]
    public Button rightButton;
    public Button leftButton;

    [Header("content")]
    public List<TutorialClip> contents = new List<TutorialClip>();
    public bool isInit;
    
    private int curIdx;
    private int pageLength;

    public void SetValue()
    {
        videoPlayer.clip = contents[curIdx].videoClip;
        descriptionText.text = contents[curIdx].description;
        indexText.text = (curIdx + 1).ToString() + "/" + pageLength.ToString();
    }

    public void NextPage(bool isRight)
    {
        int sign = isRight ? 1 : -1;
        curIdx += sign;

        SetButton();
    }

    private void SetButton()
    {
        rightButton.gameObject.SetActive(true);
        leftButton.gameObject.SetActive(true);

        if (isInit)
        {
            if (curIdx == 0)
            {
                leftButton.gameObject.SetActive(false);
            }
            if (curIdx == pageLength)
            {
                UIManager.instance.notifyObserver(EventState.None);
                return;
            }
        }
        else
        {
            curIdx = (curIdx + pageLength) % pageLength;
        }

        SetValue();
    }

    public void Notify(EventState state)
    {
        if (state == EventState.Tutorial)
        {
            InputManager.SetCanInput(false);
            gameObject.SetActive(true);
            curIdx = 0;
            pageLength = contents.Count;
            SetButton();

            AkSoundEngine.SetRTPCValue("CLICK", 2);
            AkSoundEngine.PostEvent("Play_Tutorial_Pop_up", gameObject);
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            InputManager.SetCanInput(true);
            AkSoundEngine.SetRTPCValue("CLICK", 1);

            if (!GameManager.instance.isLoad && isInit)
                UIManager.instance.notifyObserver(EventState.CityLevelUp);
        }
    }
}
