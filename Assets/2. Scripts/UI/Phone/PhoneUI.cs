using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Dictionary<EventState, PhoneState> matchState = new Dictionary<EventState, PhoneState>();
    private EventState curState;

    private void Start()
    {
        matchState[EventState.Phone] = PhoneState.Main;
        matchState[EventState.PayFail] = PhoneState.Credit;
        matchState[EventState.CityLevelUp] = PhoneState.Level;
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(550.0f, transform.localPosition.y, transform.localPosition.z);
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
        transform.DOLocalMoveX(250.0f, moveTime);

        yield return new WaitForSeconds(moveTime + waitTime);

        ChaneState(matchState[curState]);
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

    public void AddMail(MailType type)
    {
        GameObject mail = Instantiate(mailPrefabs[(int)type], mailParent);
        mail.transform.SetAsFirstSibling();
    }

    public void Notify(EventState state)
    {
        curState = state;

        if (state == EventState.Phone || state == EventState.PayFail || state == EventState.CityLevelUp)
        {
            gameObject.SetActive(true);

            if (state == EventState.CityLevelUp)
                AddMail(MailType.Level);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
