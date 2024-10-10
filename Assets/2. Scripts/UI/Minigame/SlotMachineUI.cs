using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public enum SlotState { Before, Start, While, End }
public enum SlotType { Spade, Heart, Clover, Dia, Joker }

public class SlotMachineUI : MinigameUI
{
    [Header("Text")]
    public TextMeshProUGUI payChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI playTimesText;

    [Header("Machine")]
    public GameObject slotMachine;
    public GameObject lever;
    public GameObject[] slots;

    [Header("Slot")]
    public MeshRenderer[] slotMeshs;
    public SlotState state;

    [Header("Light")]
    public Light[] jackpotLights;
    public Color[] jackpotLightColors;
    public MeshRenderer[] jackpotLightMeshs;
    public Light[] redLights;
    public MeshRenderer[] redLightMeshs;

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
    private SlotType[] ranSlot = new SlotType[3];

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        SetValue();
        Init();
    }

    public override void SetValue()
    {
        payChipText.text = curGameBuilding.betChip.ToString();
        curChipText.text = ChipManager.instance.curChip.ToString();
        playTimesText.text = curGameBuilding.betTimes.ToString();
    }

    private IEnumerator SpinSlot(int idx)
    {
        int slotLength = System.Enum.GetValues(typeof(SlotType)).Length;
        isEndSlot[idx] = false;

        float rotateSecond = oneSlotSecond * slotLength * prevRotateNum;
        float rotateAngle = 360.0f * prevRotateNum;
        if (slotIdx[idx] > 2)
            rotateAngle *= -1;
        slots[idx].transform.DOLocalRotate(slots[idx].transform.localEulerAngles + Vector3.right * rotateAngle * (idx + 1), rotateSecond * (idx + 1), RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(rotateSecond * (idx + 1));

        while (true)
        {
            float angle = 360.0f / slotLength;
            if (slotIdx[idx] > 2)
                angle *= -1;
            slots[idx].transform.DOLocalRotate(slots[idx].transform.localEulerAngles + Vector3.right * angle, oneSlotSecond, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            slotIdx[idx] = (slotIdx[idx] + 1) % slotLength;

            if (slotIdx[idx] == (int)ranSlot[idx])
            {
                if (idx == 0)
                    break;
                else if (isEndSlot[idx - 1])
                    break;
            }

            yield return new WaitForSeconds(oneSlotSecond);
        }

        AkSoundEngine.PostEvent("Play_Slot_Stop_01", gameObject);
        isEndSlot[idx] = true;

        if (idx == slots.Length - 1)
        {
            AkSoundEngine.PostEvent("Stop_Slot_Drrrrrr_01", gameObject);

            for (int i = 0; i < isDuplicate.Length; i++)
            {
                if (isDuplicate[i])
                {
                    redLights[i].gameObject.SetActive(true);
                    redLightMeshs[i].material.color = Color.red;
                }
            }

            if (isDuplicate[0] && isDuplicate[1] && isDuplicate[2])
            {
                ApplyResult();
                StartCoroutine(PlayJackpotLight());
            }

            yield return new WaitForSeconds(resultOnSecond);

            state = SlotState.End;
        }
    }

    private IEnumerator PlayJackpotLight()
    {
        int cnt = 0;

        for (int i = 0; i < jackpotLights.Length; i++)
        {
            jackpotLights[i].gameObject.SetActive(true);
        }

        AkSoundEngine.PostEvent("Play_Slotmashin_Jackpot_Ani_01", gameObject);

        while (jackpotLights[0].gameObject.activeSelf)
        {
            cnt++;

            for (int i = 0; i < jackpotLights.Length; i++)
            {
                int idx = (i + cnt) % jackpotLightColors.Length;
                jackpotLightMeshs[i].material.color = jackpotLightColors[idx];
                jackpotLights[i].color = jackpotLightColors[idx];
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ApplyResult()
    {
        ChipManager.instance.curChip += curGameBuilding.betChip * ((int)ranSlot[0] + 1);

        if (ranSlot[0] == SlotType.Joker)
            ChipManager.instance.curChip += curGameBuilding.betChip;

        SetValue();
    }

    private void RollRandom()
    {
        int slotLength = System.Enum.GetValues(typeof(SlotType)).Length;
        
        for (int i = 0;i < slotLength; i++)
            ranSlot[i] = (SlotType)Random.Range(0, slotLength);

        FindDuplicate(ranSlot);
    }

    public void FindDuplicate(SlotType[] slots)
    {
        for (int i = 0; i < isDuplicate.Length; i++)
        {
            isDuplicate[i] = false;
        }

        if (slots[0] == slots[1] && slots[1] == slots[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            isDuplicate[2] = true;
        }
        else if (slots[0] == slots[1])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
        }
        else if (slots[0] == slots[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            SlotType temp = slots[2];
            slots[2] = slots[1];
            slots[1] = temp;
        }
        else if (slots[1] == slots[2])
        {
            isDuplicate[0] = true;
            isDuplicate[1] = true;
            SlotType temp = slots[2];
            slots[2] = slots[0];
            slots[0] = temp;
        }
    }

    private void StartSlot()
    {
        if (curGameBuilding.betTimes == 0)
        {
            //TODO - ErrorMsg
            return;
        }
        else if (!ChipManager.instance.PayChip(curGameBuilding.betChip))
        {
            //TODO - ErrorMsg
            return;
        }

        curGameBuilding.betTimes--;
        SetValue();

        RollRandom();
        InitSlot();
        PlayLeverAnim();
        state = SlotState.While;

        AkSoundEngine.PostEvent("Play_Slot_Drrrrrr_01", gameObject);
        AkSoundEngine.PostEvent("Stop_Slotmashin_Jackpot_Ani_01", gameObject);
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

        AkSoundEngine.PostEvent("Play_Slot_Rever_01", gameObject);
    }

    private void InitSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            redLights[i].gameObject.SetActive(false);
            redLightMeshs[i].material.color = Color.red / 2;
        }

        for (int i = 0; i < jackpotLights.Length; i++)
        {
            jackpotLights[i].gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        InputManager.SetCanInput(false);
        state = SlotState.Before;
        slotMachine.transform.localPosition = new Vector3(530, slotMachine.transform.localPosition.y, slotMachine.transform.localPosition.z);
        slotMachine.transform.DOLocalMoveX(220, slotmachineOnSecond).OnComplete(() => { state = SlotState.Start; });
        InitSlot();

        AkSoundEngine.PostEvent("Play_Slot_Spwan_01", gameObject);
        AkSoundEngine.SetRTPCValue("CLICK", 2);
        AkSoundEngine.SetRTPCValue("INDEX1", -1);
        AkSoundEngine.SetRTPCValue("INDEX2", -1);
    }

    private void OnDisable()
    {
        InputManager.SetCanInput(true);
        AkSoundEngine.SetRTPCValue("CLICK", 1);
        AkSoundEngine.PostEvent("Stop_Slotmashin_Jackpot_Ani_01", gameObject);
    }
}
