using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PhoneState { Main, Bank, Mail, Credit, Level }
public enum MailType { Credit, Level }

public class PhoneUI : MonoBehaviour, IObserver
{
    public PhoneState state;

    [Header("Panel")]
    public GameObject[] panels;

    [Header("Time")]
    public float moveTime;
    public float waitTime;

    [Header("Mail")]
    public Transform mailParent;
    public GameObject[] mailPrefabs;

    public PanelData prevData;

    private Dictionary<EventState, PhoneState> matchState = new Dictionary<EventState, PhoneState>();
    private EventState curState;
    private bool isTweening;

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
    }

    IEnumerator InitPhone()
    {
        transform.DOLocalMoveY(0.0f, moveTime).SetEase(Ease.OutBack);
        InputManager.SetCanInput(false);

        yield return new WaitForSeconds(moveTime + waitTime);

        ChaneState(matchState[curState]);

        if (curState == EventState.CityLevelUp && CityLevelManager.instance.levelIdx == 1)
        {
            yield return new WaitForSeconds(1.0f);
            UIManager.instance.notifyObserver(EventState.None);
        }
        else
            InputManager.SetCanInput(true);
    }

    public void ChaneState(PhoneState state)
    {
        this.state = state;

        for(int i = 0;i < panels.Length; i++)
        {
            if ((PhoneState)i != state)
                panels[i].SetActive(false);
        }

        panels[(int)state].SetActive(true);
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
            data = new LevelPanelData(CityLevelManager.instance.levelIdx);
        }

        SetPanelData(state, data);
    }

    public void AddMail(MailType type, PanelData data)
    {
        MailUI mail = Instantiate(mailPrefabs[(int)type], mailParent).GetComponent<MailUI>();
        mail.transform.SetAsFirstSibling();
        mail.SetValue();

        Button button = mail.gameObject.GetComponent<Button>();

        if (button != null)
        {
            PhoneState state = (PhoneState)type + 3;
            button.onClick.AddListener(() => SetPanelData((PhoneState)(type + 3), data));
            button.onClick.AddListener(() => ChaneState((PhoneState)(type + 3)));
            button.onClick.AddListener(() => ReturnToCurrentData((PhoneState)(type + 3)));
            button.onClick.AddListener(() => PlayButtonClickSound());
        }
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
                PanelData data = new LevelPanelData(CityLevelManager.instance.levelIdx);
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
                    UIManager.instance.notifyObserver(EventState.SocialEffect);
                }

                gameObject.SetActive(false);
                isTweening = false;
            });
        }
    }
}
