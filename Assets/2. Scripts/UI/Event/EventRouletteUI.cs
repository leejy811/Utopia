using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RouletteState { Before, Start, While, End }

public class EventRouletteUI : MonoBehaviour, IObserver
{
    [Header("Result")]
    public SlotResultUI[] slotResults;

    [Header("Slot")]
    public Material[] slotMats;
    public RouletteState state;

    [Header("Light")]
    public Light[] jackpotLights;
    public Color[] jackpotLightColors;
    public Material[] jackpotLightMats;
    public Light[] redLights;
    public Material[] redLightMats;

    [Header("Object")]
    //public GameObject costPanel;
    public GameObject slotMachine;
    public GameObject lever;
    public GameObject[] slots;

    [Header("Cost")]
    public TextMeshProUGUI costText;

    [Header("Parameter")]
    public float slotmachineOnSecond;
    public float oneSlotSecond;
    public float resultOnSecond;
    public float resultOffSecond;
    public float leverDownSecond;
    public float leverUpSecond;
    public int prevRotateNum;

    private bool[] isDuplicate = new bool[3];
    private bool[] isEndSlot = new bool[3];

    private int[] slotIdx = new int[3];
    private int[] ranIdx;
    private Event[] possibleEvents;

    private IEnumerator SpinSlot(int idx)
    {
        isEndSlot[idx] = false;

        float rotateSecond = oneSlotSecond * possibleEvents.Length * prevRotateNum;
        float rotateAngle = 360.0f * prevRotateNum;
        if (slotIdx[idx] > 3)
            rotateAngle *= -1;
        slots[idx].transform.DOLocalRotate(slots[idx].transform.localEulerAngles + Vector3.right * rotateAngle, rotateSecond, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(rotateSecond);

        while (true)
        {
            float angle = 360.0f / possibleEvents.Length;
            if (slotIdx[idx] > 3)
                angle *= -1;
            slots[idx].transform.DOLocalRotate(slots[idx].transform.localEulerAngles + Vector3.right * angle, oneSlotSecond, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            slotIdx[idx] = (slotIdx[idx] + 1) % possibleEvents.Length;

            if (slotIdx[idx] == ranIdx[idx])
            {
                if (idx == 0)
                    break;
                else if (isEndSlot[idx - 1])
                    break;
            }

            yield return new WaitForSeconds(oneSlotSecond);
        }

        slotResults[idx].SetValue(possibleEvents[ranIdx[idx]], resultOnSecond);
        isEndSlot[idx] = true;

        if (idx == slots.Length - 1)
        {
            EventManager.instance.EffectUpdate();

            for (int i = 0; i < isDuplicate.Length; i++)
            {
                if (isDuplicate[i])
                {
                    redLights[i].gameObject.SetActive(true);
                    redLightMats[i].color = Color.red;
                }
            }

            if (isDuplicate[0] && isDuplicate[1] && isDuplicate[2])
                StartCoroutine(PlayJackpotLight());

            yield return new WaitForSeconds(resultOnSecond);

            state = RouletteState.End;
        }
    }

    private IEnumerator PlayJackpotLight()
    {
        int cnt = 0;

        for (int i = 0; i < jackpotLights.Length; i++)
        {
            jackpotLights[i].gameObject.SetActive(true);
        }

        //���� ��鸲 �ִϸ��̼�

        while (jackpotLights[0].gameObject.activeSelf)
        {
            cnt++;

            for (int i = 0; i < jackpotLights.Length; i++)
            {
                int idx = (i + cnt) % jackpotLightColors.Length;
                jackpotLightMats[i].color = jackpotLightColors[idx];
                jackpotLights[i].color = jackpotLightColors[idx];
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnButtonClick()
    {
        if (state == RouletteState.End)
        {
            StartCoroutine(ReStartSlot());
            return;
        }

        StartSlot();
    }

    private void StartSlot()
    {
        PlayLeverAnim();
        state = RouletteState.While;

        for (int i = 0; i < slots.Length; i++)
        {
            StartCoroutine(SpinSlot(i));
        }
    }

    private void PlayLeverAnim()
    {
        lever.transform.DOLocalRotate(new Vector3(120, 0, 0), leverDownSecond, RotateMode.Fast)
            .OnComplete(() => 
            { 
                lever.transform.DOLocalRotate(new Vector3(0, 0, 0), leverUpSecond, RotateMode.Fast)
                    .SetEase(Ease.InOutSine);
            }
            )
            .SetEase(Ease.InOutSine);
    }

    private IEnumerator ReStartSlot()
    {
        state = RouletteState.While;
        InitSlot();

        for (int i = slotResults.Length - 1; i >= 0; i--)
        {
            slotResults[i].ResetPosition(resultOffSecond);
            yield return new WaitForSeconds(resultOffSecond / 2.0f);
        }

        StartSlot();
    }

    public void SetRandomEvent(int[] idx)
    {
        ranIdx = idx;

        for (int i = 0; i < isDuplicate.Length; i++)
        {
            isDuplicate[i] = false;
        }

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
        for (int i = 0; i < slotMats.Length; i++)
        {
            slotMats[i].mainTexture = events[i].eventTexture;
        }

        possibleEvents = events;
    }

    private void InitSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            redLights[i].gameObject.SetActive(false);
            redLightMats[i].color = Color.red / 2;
        }

        for (int i = 0; i < jackpotLights.Length; i++)
        {
            jackpotLights[i].gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        InputManager.canInput = false;
        RoutineManager.instance.OnOffDailyLight(false);
        state = RouletteState.Before;
        slotMachine.transform.localPosition = new Vector3(530, slotMachine.transform.localPosition.y, slotMachine.transform.localPosition.z);
        //costPanel.transform.localPosition = new Vector3(600, costPanel.transform.localPosition.y, costPanel.transform.localPosition.z);
        slotMachine.transform.DOLocalMoveX(220, slotmachineOnSecond)
            .OnComplete(() =>
            {
                state = RouletteState.Start;
                //costPanel.transform.DOLocalMoveX(200, slotmachineOnSecond)
                        //.OnComplete(() => { state = RouletteState.Start; }); 
            });
        InitSlot();
        costText.text = GetCommaText(EventManager.instance.cost);
    }

    private void OnDisable()
    {
        InputManager.canInput = true;
        RoutineManager.instance.OnOffDailyLight(true);
    }

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
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
