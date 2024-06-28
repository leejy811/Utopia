using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RouletteState { Start, While, End }

public class EventRouletteUI : MonoBehaviour, IObserver
{
    [Header("Text")]
    public TextMeshProUGUI[] eventNameText;
    public TextMeshProUGUI[] eventEffectText;
    public Image[] lightImage;

    [Header("Slot")]
    public Slot[] slots;
    public float spinSpeed;
    public float distanceMultiplier;
    public float returnPositionMultiplier;
    public float initPosY;
    public RouletteState state;

    private bool[] isDuplicate = new bool[3];
    private bool[] isEndSlot = new bool[3];

    private int[] ranIdx;
    private Event[] possibleEvents;

    private IEnumerator SpinSlot(int idx)
    {
        GameObject slotContainer = slots[idx].gameObject;
        float reachedDistance = (possibleEvents.Length - (slots[idx].transform.localPosition.y / distanceMultiplier) + ranIdx[idx]) * distanceMultiplier;
        float movedDistance = 0;

        while (movedDistance < reachedDistance)
        {
            movedDistance += spinSpeed * Time.fixedDeltaTime;
            slotContainer.transform.localPosition += Vector3.up * spinSpeed * Time.fixedDeltaTime;

            if (slotContainer.transform.localPosition.y >= (slots[idx].images.Length - 1) * distanceMultiplier + initPosY)
            {
                Vector3 pos = slotContainer.transform.localPosition;
                pos.y -= (slots[idx].images.Length - 1) * distanceMultiplier;
                slotContainer.transform.localPosition = pos;
            }

            if (idx != 0 && movedDistance >= reachedDistance && !isEndSlot[idx - 1])
                reachedDistance += possibleEvents.Length * distanceMultiplier;

            yield return new WaitForFixedUpdate();
        }

        eventNameText[idx].text = possibleEvents[ranIdx[idx]].eventName;
        eventEffectText[idx].text = possibleEvents[ranIdx[idx]].eventEffectComment;
        isEndSlot[idx] = true;

        if (idx == slots.Length - 1)
        {
            state = RouletteState.End;
            for (int i = 0; i < slots.Length; i++)
                lightImage[i].color = isDuplicate[i] ? Color.red : Color.gray;

            EventManager.instance.EffectUpdate();
        }
    }

    public void OnButtonClick()
    {
        gameObject.GetComponentInChildren<Animator>().SetTrigger("DoLever");
        ProcessButtonClick();
    }

    public void SetRandomEvent(int[] idx)
    {
        InitSlot();
        ranIdx = idx;

        if (ranIdx[0] == ranIdx[1] && ranIdx[1] == ranIdx[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            isDuplicate[2] = true;
        }
        else if (ranIdx[0] == ranIdx[1])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
        }
        else if (ranIdx[0] == ranIdx[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            int temp = ranIdx[2];
            ranIdx[2] = ranIdx[1];
            ranIdx[1] = temp;
        }
        else if (ranIdx[1] == ranIdx[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            int temp = ranIdx[2];
            ranIdx[2] = ranIdx[0];
            ranIdx[0] = temp;
        }
    }

    public void SetPossibleEvent(Event[] events)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSlot(events);
        }

        possibleEvents = events;
    }

    private void ProcessButtonClick()
    {
        state = RouletteState.While;

        for (int i = 0; i < slots.Length; i++)
        {
            StartSlot(i);
        }
    }

    public void StartSlot(int index)
    {
        StartCoroutine(SpinSlot(index));
    }

    private void InitSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            eventNameText[i].text = "??????????????";
            eventEffectText[i].text = "??????????????";
            lightImage[i].color = Color.gray;
            isDuplicate[i] = false;
            isEndSlot[i] = false;
            //slots[i].transform.localPosition = new Vector3(slots[i].transform.localPosition.x, initPosY, slots[i].transform.localPosition.z);
        }

        state = RouletteState.Start;
    }

    public void OnEnable()
    {
        InitSlot();
    }

    private void OnDisable()
    {
        InputManager.canInput = true;
    }

    public void Notify(EventState state)
    {
        if(state == EventState.SlotMachine)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
    }
}
