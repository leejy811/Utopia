using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct TutorialContent
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
    public TutorialContent[] gameStart;
    public TutorialContent[] constructButton;
    public TutorialContent[] constructBuilding;
    public TutorialContent[] socialEffect;

    private Dictionary<EventState, TutorialContent[]> content = new Dictionary<EventState, TutorialContent[]>();
    private EventState curState;
    public int curIdx;
    private int pageLength;

    private void Awake()
    {
        content[EventState.GameStart] = gameStart;
        content[EventState.ConstructButton] = constructButton;
        content[EventState.ConstructBuilding] = constructBuilding;
        content[EventState.SocialEffect] = socialEffect;
    }

    public void SetValue()
    {
        videoPlayer.clip = content[curState][curIdx].videoClip;
        descriptionText.text = content[curState][curIdx].description;
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
        rightButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "¥Ÿ¿Ω";
        leftButton.interactable = true;

        if (curIdx == 0)
        {
            leftButton.interactable = false;
        }
        if (curIdx == pageLength - 1)
        {
            rightButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "¥›±‚";
        }
        if (curIdx == pageLength)
        {
            UIManager.instance.notifyObserver(EventState.None);
            return;
        }

        SetValue();
    }

    public void Notify(EventState state)
    {
        if (state == EventState.GameStart || state == EventState.ConstructButton ||
            state == EventState.ConstructBuilding || state == EventState.SocialEffect)
        {
            gameObject.SetActive(true);
            curState = state;
            curIdx = 0;
            pageLength = content[curState].Length;
            SetButton();
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            InputManager.SetCanInput(true);

            if (curState == EventState.GameStart)
                UIManager.instance.notifyObserver(EventState.CityLevelUp);
        }
    }
}
