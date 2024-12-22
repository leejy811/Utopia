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

[System.Serializable]
public struct TutorialState
{
    public EventState state;
    public TutorialClip[] clips;
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
    public List<TutorialState> content = new List<TutorialState>();
    
    private EventState curState;
    private int curIdx;
    private int pageLength;

    public void SetValue()
    {
        videoPlayer.clip = FindState(curState).clips[curIdx].videoClip;
        descriptionText.text = FindState(curState).clips[curIdx].description;
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

        if (curIdx == 0)
        {
            leftButton.gameObject.SetActive(false);
        }
        if (curIdx == pageLength - 1)
        {
            //rightButton.gameObject.SetActive(false);
        }
        if (curIdx == pageLength)
        {
            UIManager.instance.notifyObserver(EventState.None);
            return;
        }

        SetValue();
    }

    public TutorialState FindState(EventState state)
    {
        return content.Find(x => x.state == state);
    }

    public bool ContainState(EventState state)
    {
        return content.Exists(x => x.state == state);
    }

    public void Notify(EventState state)
    {
        if (ContainState(state))
        {
            InputManager.SetCanInput(false);
            gameObject.SetActive(true);
            curState = state;
            curIdx = 0;
            pageLength = FindState(curState).clips.Length;
            SetButton();

            AkSoundEngine.SetRTPCValue("CLICK", 2);
            AkSoundEngine.PostEvent("Play_Tutorial_Pop_up", gameObject);
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            InputManager.SetCanInput(true);
            AkSoundEngine.SetRTPCValue("CLICK", 1);

            if (curState == EventState.GameStart && !GameManager.instance.isLoad)
                UIManager.instance.notifyObserver(EventState.CityLevelUp);
        }
    }
}
