using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum MinigameState { Lobby, Betting, Play, Result }

public class MinigameUI : MonoBehaviour, IObserver
{
    [Header("Panel")]
    public GameObject[] panels;

    [Header("Chip")]
    public TextMeshProUGUI payChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI playTimesText;
    public TextMeshProUGUI errorMsgText;

    protected EnterBuilding curGameBuilding;

    public virtual void InitGame(EnterBuilding building)
    {
        gameObject.SetActive(true);
        curGameBuilding = building;
        InputManager.SetCanInput(false);
        SetState(MinigameState.Lobby);
        SetValue();

        AkSoundEngine.SetRTPCValue("CLICK", 2);
        AkSoundEngine.SetRTPCValue("INDEX1", -1);
        AkSoundEngine.SetRTPCValue("INDEX2", -1);
    }

    protected virtual void SetValue()
    {
        payChipText.text = curGameBuilding.betChip.ToString();
        curChipText.text = ChipManager.instance.curChip.ToString();
        playTimesText.text = curGameBuilding.betTimes.ToString();
    }

    protected void SetErrorMsg(string message, float second)
    {
        errorMsgText.color += Color.black;
        errorMsgText.text = message;
        errorMsgText.DOFade(0.0f, second);
    }

    protected virtual void SetUI(MinigameState state)
    {

    }

    protected virtual void SetState(MinigameState state)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == (int)state);
        }

        SetUI(state);
    }

    protected virtual void OnDisable()
    {
        InputManager.SetCanInput(true);

        UIManager.instance.topPanel.SetActive(true);
        UIManager.instance.bottomPanel.SetActive(true);
    }

    public void Notify(EventState state)
    {
        if (state != EventState.Minigame)
            gameObject.SetActive(false);
    }
}
