using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RouletteState { Start, While, End }

public class EventRouletteUI : MonoBehaviour
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

    private float[] slotsDistance = new float[3];
    private float[] slotsRand = new float[3];
    private bool[] isDuplicate = new bool[3];

    private Event[] ranEvents;

    private IEnumerator SpinSlot(int idx)
    {
        GameObject slotContainer = slots[idx].gameObject;
        float reachedDistance = slotsRand[idx];
        float movedDistance = 0;

        while (movedDistance < reachedDistance * distanceMultiplier)
        {
            slotContainer.transform.localPosition += Vector3.up * spinSpeed * Time.fixedDeltaTime;

            if (slotContainer.transform.localPosition.y >= (slots[idx].images.Length - 1) * distanceMultiplier - 50)
            {
                Vector3 pos = slotContainer.transform.localPosition;
                pos.y -= (slots[idx].images.Length - 1) * distanceMultiplier;
                slotContainer.transform.localPosition = pos;
            }

            movedDistance += spinSpeed * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        eventNameText[idx].text = ranEvents[idx].eventName;
        eventEffectText[idx].text = ranEvents[idx].eventEffectComment;

        if(idx == slots.Length - 1)
        {
            state = RouletteState.End;
            for (int i = 0; i < slots.Length; i++)
                lightImage[i].color = isDuplicate[i] ? Color.red : Color.gray;
        }
    }

    public void OnButtonClick()
    {
        gameObject.GetComponentInChildren<Animator>().SetTrigger("DoLever");
        ProcessButtonClick();
    }

    public void SetEvent(Event[] events)
    {
        ranEvents = events;

        for (int i = 0; i < slots.Length; i++)
        {
            slotsDistance[i] = events[i].eventIndex;
            slotsRand[i] = Random.Range((i + 1) * 2 + 1, (i + 1) * 2 + 3);

            slots[i].transform.localPosition += Vector3.up * distanceMultiplier * (slotsDistance[i] - slotsRand[i]);

            if (slots[i].transform.localPosition.y <= 0)
            {
                Vector3 pos = slots[i].transform.localPosition;
                pos.y += (slots[i].images.Length - 1) * distanceMultiplier;
                slots[i].transform.localPosition = pos;
            }
        }

        if (ranEvents[0] == ranEvents[1] && ranEvents[1] == ranEvents[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            isDuplicate[2] = true;
        }
        else if (ranEvents[0] == ranEvents[1])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
        }
        else if (ranEvents[0] == ranEvents[2])
        {
            isDuplicate[0] = true;
            isDuplicate[2] = true;
        }
        else if (ranEvents[1] == ranEvents[2])
        {
            isDuplicate[1] = true;
            isDuplicate[2] = true;
        }
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

    public void OnEnable()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            eventNameText[i].text = "";
            eventEffectText[i].text = "";
            lightImage[i].color = Color.gray;
            isDuplicate[i] = false;
            slots[i].transform.localPosition = new Vector3(slots[i].transform.localPosition.x, initPosY, slots[i].transform.localPosition.z);
        }

        state = RouletteState.Start;
    }

    private void OnDisable()
    {
        InputManager.canInput = true;
    }
}
