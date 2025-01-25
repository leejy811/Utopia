using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    [Header("Opening")]
    public Animator openingAnim;
    public GameObject gamePanel;
    public float openingSecond;
    public float openingInterval;
    public string openingSound;
    public bool postProcessing;
    public UniversalAdditionalCameraData uiCamera;

    [Header("AddChip")]
    public Vector2 chipImageBox;
    public Vector2 plusLimitBox;
    public float plusMoveDist;
    public float plusScale;
    public float plusSecond;
    public string poolName;

    protected EnterBuilding curGameBuilding;

    public virtual void InitGame(EnterBuilding building)
    {
        gameObject.SetActive(true);
        StartCoroutine(PlayOpening(true));
        curGameBuilding = building;
        InputManager.SetCanInput(false);
        SetState(MinigameState.Lobby);
        SetValue();

        AkSoundEngine.SetRTPCValue("INDEX1", -1);
        AkSoundEngine.SetRTPCValue("INDEX2", -1);
    }

    protected virtual void SetValue()
    {
        payChipText.text = ((int)curGameBuilding.values[ValueType.betChip].cur).ToString();
        curChipText.text = ChipManager.instance.CurChip.ToString();
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

    public virtual void OnClickCloseGame()
    {
        StartCoroutine(PlayOpening(false));
    }

    protected IEnumerator PlayOpening(bool isOpen)
    {
        if (openingAnim != null) 
        {
            if(isOpen)
                UIManager.instance.MovePanelAnim(openingSecond, true);

            openingAnim.gameObject.SetActive(true);
            openingAnim.SetFloat("Speed", 1.0f / openingSecond);
            openingAnim.SetBool("IsPlaying", false);
            AkSoundEngine.PostEvent(openingSound, gameObject);
            yield return new WaitForSeconds(openingSecond);

            if (postProcessing)
                uiCamera.renderPostProcessing = isOpen;

            yield return new WaitForSeconds(openingInterval);

            SetGamePanel(isOpen);
            openingAnim.SetBool("IsPlaying", true);
            AkSoundEngine.PostEvent(openingSound, gameObject);

            yield return new WaitForSeconds(openingSecond);

            openingAnim.gameObject.SetActive(false);

            if (!isOpen)
            {
                UIManager.instance.MovePanelAnim(openingSecond, false);
                UIManager.instance.notifyObserver(EventState.None);
            }
        }
        else
        {
            gamePanel.SetActive(isOpen);
            if (!isOpen)
                UIManager.instance.notifyObserver(EventState.None);
        }
    }

    protected virtual void SetGamePanel(bool active)
    {
        gamePanel.SetActive(active);
        AkSoundEngine.SetRTPCValue("MINIGAME", active ? 2 : 1);
    }

    protected IEnumerator PlayAddChip(float second, int chip)
    {
        RectTransform plus = PoolSystem.instance.messagePool.GetFromPool<RectTransform>(poolName);
        TextMeshProUGUI plusText = plus.GetComponent<TextMeshProUGUI>();

        plus.localScale = Vector3.one * plusScale;
        float xPos = Random.Range(chipImageBox.x, plusLimitBox.x);
        float yPos = Random.Range(chipImageBox.y, plusLimitBox.y);
        int xSign = Random.Range(0, 2) == 0 ? -1 : 1;
        int ySign = Random.Range(0, 2) == 0 ? -1 : 1;
        plus.localPosition = new Vector3(xPos * xSign, yPos * ySign, 0);
        plus.DOLocalMove(plus.localPosition + plus.localPosition.normalized * plusMoveDist, second);

        plusText.text = (chip >= 0 ? "+" : "-") + Mathf.Abs(chip).ToString();
        plusText.color += Color.black;
        plusText.DOFade(0.0f, second);

        yield return new WaitForSeconds(second);

        PoolSystem.instance.messagePool.TakeToPool<RectTransform>(poolName, plus);
    }

    public void Notify(EventState state)
    {
        if (state != EventState.Minigame)
            gameObject.SetActive(false);
    }
}
