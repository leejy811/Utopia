using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum PhoneState { Main, Bank, Mail, Credit, Level, Chip, GameList }
public enum MailType { Credit, Level }

public class PhoneUI : MonoBehaviour, IObserver
{
    public PhoneState state;

    [Header("Panel")]
    public GameObject[] panels;

    [Header("Time")]
    public float moveTime;
    public float waitTime;
    public float turnTime;

    [Header("Mail")]
    public Transform mailParent;
    public GameObject[] mailPrefabs;

    [Header("Device")]
    public Transform mainDevice;
    public Transform colDevice;
    public Transform rowDevice;

    public List<MailData> mailDatas = new List<MailData>();
    public PanelData prevData;

    private Dictionary<EventState, PhoneState> matchState = new Dictionary<EventState, PhoneState>();
    private EventState curState;
    private bool isTweening;
    private bool isRow;

    private void Start()
    {
        matchState[EventState.Phone] = PhoneState.Main;
        matchState[EventState.PayFail] = PhoneState.Credit;
        matchState[EventState.CityLevelUp] = PhoneState.Level;
        matchState[EventState.DebtDoc] = PhoneState.Bank;
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -450.0f, transform.localPosition.z);
        StartCoroutine(InitPhone());
    }

    private void OnDisable()
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        ChangeToDevice(false, 0.0f);
    }

    IEnumerator InitPhone()
    {
        transform.DOLocalMoveY(-80.0f, moveTime).SetEase(Ease.OutBack);
        InputManager.SetCanInput(false);

        yield return new WaitForSeconds(moveTime + waitTime);

        ChaneState(matchState[curState]);

        if (curState == EventState.CityLevelUp && CityLevelManager.instance.levelIdx == 1
            && !GameManager.instance.skipTutorial && UIManager.instance.tutorial.ContainState(EventState.CityLevelUp))
        {
            yield return new WaitForSeconds(5.0f);
            UIManager.instance.notifyObserver(EventState.None);
        }
        else
            InputManager.SetCanInput(true);
    }

    public void ChaneState(PhoneState state)
    {
        PhoneState prevState = this.state;
        this.state = state;

        for(int i = 0;i < panels.Length; i++)
        {
            if ((PhoneState)i != state)
                panels[i].SetActive(false);
        }

        panels[(int)state].SetActive(true);

        float second = state == PhoneState.Chip || prevState == PhoneState.Chip ? turnTime : 0.0f;
        ChangeToDevice(state == PhoneState.Chip, second);
    }

    public void ChangeStateToInt(int state)
    {
        ChaneState((PhoneState)state);
    }

    public void SetPanelData(PhoneState state, PanelData data)
    {
        panels[(int)state].GetComponent<PanelUI>().data = data;
    }

    public void ReturnToCurrentData(PhoneState state)
    {
        PanelData data = null;

        if (state == PhoneState.Credit)
        {
            data = prevData;
        }
        else if (state == PhoneState.Level)
        {
            data = new LevelPanelData(CityLevelManager.instance.levelIdx, DateTime.Now);
        }

        SetPanelData(state, data);
    }

    public MailUI AddMail(MailType type, PanelData data)
    {
        MailData mailData = new MailData();
        if (type == MailType.Credit)
        {
            mailData.Save(type, data as CreditPanelData, false);
        }
        else if (type == MailType.Level)
        {
            mailData.Save(type, data as LevelPanelData, false);
        }
        mailDatas.Add(mailData);

        MailUI mail = Instantiate(mailPrefabs[(int)type], mailParent).GetComponent<MailUI>();
        mail.transform.SetAsFirstSibling();
        mail.SetValue(mailData);

        Button button = mail.gameObject.GetComponent<Button>();

        if (button != null)
        {
            PhoneState state = (PhoneState)type + 3;
            button.onClick.AddListener(() => SetPanelData((PhoneState)(type + 3), data));
            button.onClick.AddListener(() => ChaneState((PhoneState)(type + 3)));
            button.onClick.AddListener(() => ReturnToCurrentData((PhoneState)(type + 3)));
            button.onClick.AddListener(() => PlayButtonClickSound());
            button.onClick.AddListener(() => SetMailClickData(mailDatas.Count - 1));
        }

        return mail;
    }

    public void LoadMail(MailData data)
    {
        data.creditPanelData.day = data.mailDay;
        data.levelPanelData.day = data.mailDay;
        MailUI mail = AddMail(data.mailType, data.mailType == MailType.Credit ? data.creditPanelData : data.levelPanelData);
        mail.gameObject.GetComponent<Button>().interactable = !data.isClick;
        mail.newImage.gameObject.SetActive(!data.isClick);
    }

    public void SetMailClickData(int index)
    {
        mailDatas[index].isClick = true;
    }

    public void PlayButtonClickSound()
    {
        AkSoundEngine.PostEvent("Play_CellPhone_Click", gameObject);
    }

    public void Notify(EventState state)
    {
        EventState prevState = curState;
        curState = state;

        if (state == EventState.Phone || state == EventState.PayFail || 
            state == EventState.CityLevelUp || state == EventState.DebtDoc)
        {
            if (state == EventState.CityLevelUp)
            {
                PanelData data = new LevelPanelData(CityLevelManager.instance.levelIdx, RoutineManager.instance.day);
                SetPanelData(PhoneState.Level, data);
                AddMail(MailType.Level, data);
            }

            gameObject.SetActive(true);

            if (state != EventState.Phone)
            {
                AkSoundEngine.PostEvent("Play_CellPhone_01", gameObject);
            }
        }
        else if (!isTweening && gameObject.activeSelf)
        {
            isTweening = true;
            transform.DOLocalMoveY(-450.0f, moveTime).SetEase(Ease.InBack).OnComplete(() => 
            {
                if (prevState == EventState.CityLevelUp && CityLevelManager.instance.levelIdx == 1)
                {
                    UIManager.instance.SetTutorialPopup(EventState.SocialEffect);
                }

                gameObject.SetActive(false);
                isTweening = false;
            });
        }
    }

    private void ChangeToDevice(bool isRow, float second)
    {
        if (mainDevice == null) return;
        isTweening = true;
        Transform targetTrans = isRow ? rowDevice : colDevice;

        mainDevice.DOLocalMove(targetTrans.localPosition, second);
        mainDevice.DOScale(targetTrans.localScale, second);
        mainDevice.DOLocalRotate(targetTrans.localEulerAngles, second).OnComplete(() =>
        {
            isTweening = false;
        });
    }
}
