using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SlotState { Ready, While }
public enum SlotType { Joker, Spade, Dia, Heart, Clover }

public class SlotMachineUI : MinigameUI
{
    [Header("Text")]
    public TextMeshProUGUI payChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI playTimesText;
    public TextMeshProUGUI errorMsgText;

    [Header("Slot")]
    public Slot[] slots;
    public SlotState state;

    [Header("Light")]
    public Light[] jackpotLights;
    public Color[] jackpotLightColors;
    public MeshRenderer[] jackpotLightMeshs;
    public Light[] redLights;
    public MeshRenderer[] redLightMeshs;

    [Header("Parameter")]
    public float oneSlotSecond;
    public float leverSecond;
    public float errorMsgSecond;
    public int prevRotateNum;
    public int[] reward;

    private bool[] isDuplicate = new bool[3];
    private bool[] isEndSlot = new bool[3];
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

        for (int i = 0;i < prevRotateNum; i++)
        {
            yield return StartCoroutine(slots[idx].RollSlot(slots[idx].curIdx, oneSlotSecond));
        }

        while (true)
        {
            if (idx == 0)
            {
                yield return StartCoroutine(slots[idx].RollSlot((int)ranSlot[idx], oneSlotSecond));
                break;
            }
            else if (isEndSlot[idx - 1])
            {
                yield return StartCoroutine(slots[idx].RollSlot((int)ranSlot[idx], oneSlotSecond));
                break;
            }
            else
            {
                for (int i = 0; i < prevRotateNum; i++)
                {
                    yield return StartCoroutine(slots[idx].RollSlot(slots[idx].curIdx, oneSlotSecond));
                }
            }
        }

        AkSoundEngine.PostEvent("Play_Slot_Stop_01", gameObject);
        isEndSlot[idx] = true;

        if (idx == slots.Length - 1)
        {
            AkSoundEngine.PostEvent("Stop_Slot_Drrrrrr_01", gameObject);

            //for (int i = 0; i < isDuplicate.Length; i++)
            //{
            //    if (isDuplicate[i])
            //    {
            //        redLights[i].gameObject.SetActive(true);
            //        redLightMeshs[i].material.color = Color.red;
            //    }
            //}

            if (isDuplicate[0] && isDuplicate[1] && isDuplicate[2])
            {
                ApplyResult();
                //StartCoroutine(PlayJackpotLight());
            }

            state = SlotState.Ready;
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
        ChipManager.instance.curChip += curGameBuilding.betChip * reward[(int)ranSlot[0]];

        SetValue();
    }

    private void RollRandom()
    {
        int slotLength = System.Enum.GetValues(typeof(SlotType)).Length;
        
        for (int i = 0;i < ranSlot.Length; i++)
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

    public void StartSlot()
    {
        if (state == SlotState.While) return;
        else if (curGameBuilding.betTimes == 0)
        {
            SetErrorMsg("실행 가능 횟수를 모두 소진하였습니다.");
            return;
        }
        else if (!ChipManager.instance.PayChip(curGameBuilding.betChip))
        {
            SetErrorMsg("칩이 부족합니다.");
            return;
        }

        curGameBuilding.betTimes--;
        SetValue();

        RollRandom();
        PlayLeverAnim();
        state = SlotState.While;

        AkSoundEngine.PostEvent("Play_Slot_Drrrrrr_01", gameObject);
        AkSoundEngine.PostEvent("Stop_Slotmashin_Jackpot_Ani_01", gameObject);
        for (int i = 0; i < slots.Length; i++)
        {
            StartCoroutine(SpinSlot(i));
        }
    }

    private void SetErrorMsg(string message)
    {
        errorMsgText.color += Color.black;
        errorMsgText.text = message;
        errorMsgText.DOFade(0.0f, errorMsgSecond);
    }

    private void PlayLeverAnim()
    {
        //Lever Animation
        AkSoundEngine.PostEvent("Play_Slot_Rever_01", gameObject);
    }

    private void Init()
    {
        InputManager.SetCanInput(false);
        state = SlotState.Ready;

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
