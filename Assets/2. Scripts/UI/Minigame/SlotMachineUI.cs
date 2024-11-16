using DG.Tweening;
using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotState { Ready, While }
public enum SlotType { Joker, Spade, Dia, Heart, Clover }

[System.Serializable]
public class SlotLight
{
    public Image lightImage;
    public Sprite onSprite;
    public Sprite offSprite;

    public void SetLight(bool isOn)
    {
        lightImage.sprite = isOn ? onSprite : offSprite;
    }
}

public class SlotMachineUI : MinigameUI
{
    [Header("Light")]
    public SlotLight[] slotLights;

    [Header("Slot")]
    public Slot[] slots;
    public SlotState state;

    [Header("Animation")]
    public Animator leverAnim;
    public Animator jackPotAnim;

    [Header("Parameter")]
    public float oneSlotSecond;
    public float leverSecond;
    public float jackPotSpeed;
    public float jackPotSecond;
    public float errorMsgSecond;
    public int prevRotateNum;
    public int[] reward;
    public Vector2Int jackPotRange;

    private bool[] isDuplicate = new bool[3];
    private bool[] isEndSlot = new bool[3];
    private SlotType[] ranSlot = new SlotType[3];
    private int curTimes;
    private int jackPotTimes;
    private Coroutine jackPotCoroutine;

    private void Start()
    {
        ResetJackPot();
        leverAnim.SetFloat("LeverSpeed", 1.0f / leverSecond);
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        Init();
    }

    protected override void SetValue()
    {
        base.SetValue();
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

            for(int i = 0;i < slotLights.Length;i++)
            {
                slotLights[i].SetLight(isDuplicate[i]);
            }

            if (isDuplicate[0] && isDuplicate[1] && isDuplicate[2])
            {
                ApplyResult();
                ResetJackPot();
                jackPotCoroutine = StartCoroutine(PlayJackPot());
            }

            state = SlotState.Ready;
        }
    }

    IEnumerator PlayJackPot()
    {
        jackPotAnim.SetBool("IsJackPot", true);
        jackPotAnim.SetFloat("LightSpeed", jackPotSpeed);

        int rewardChip = curGameBuilding.betChip * reward[(int)ranSlot[0]];
        for (int i = 0;i < rewardChip; i++)
        {
            curChipText.text = (ChipManager.instance.curChip - rewardChip + i + 1).ToString();
            StartCoroutine(PlayAddChip(plusSecond, 1));
            yield return new WaitForSeconds(jackPotSecond / rewardChip);
        }
        jackPotAnim.SetBool("IsJackPot", false);
    }

    private void StopJackPot()
    {
        if (jackPotCoroutine == null) return;

        StopCoroutine(jackPotCoroutine);
        jackPotAnim.SetBool("IsJackPot", false);
    }

    private void ApplyResult()
    {
        ChipManager.instance.curChip += curGameBuilding.betChip * reward[(int)ranSlot[0]];
    }

    private void ResetJackPot()
    {
        curTimes = 0;
        jackPotTimes = Random.Range(jackPotRange.x, jackPotRange.y);
    }

    private void RollRandom()
    {
        int slotLength = System.Enum.GetValues(typeof(SlotType)).Length;

        curTimes++;
        if(curTimes < jackPotTimes)
        {
            for (int i = 0; i < ranSlot.Length; i++)
                ranSlot[i] = (SlotType)Random.Range(0, slotLength);
        }
        else
        {
            SlotType jackpotSlot = (SlotType)Random.Range(0, slotLength);
            for (int i = 0; i < ranSlot.Length; i++)
                ranSlot[i] = jackpotSlot;
        }

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
            SetErrorMsg("실행 가능 횟수를 모두 소진하였습니다.", errorMsgSecond);
            return;
        }
        else if (!ChipManager.instance.PayChip(curGameBuilding.betChip))
        {
            SetErrorMsg("칩이 부족합니다.", errorMsgSecond);
            return;
        }

        StopJackPot();
        curGameBuilding.betTimes--;
        StartCoroutine(PlayAddChip(plusSecond, -curGameBuilding.betChip));
        SetValue();

        RollRandom();
        PlayLeverAnim();
        for (int i = 0; i < slotLights.Length; i++)
        {
            slotLights[i].SetLight(false);
        }
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
        leverAnim.SetTrigger("DoLever");
        AkSoundEngine.PostEvent("Play_Slot_Rever_01", gameObject);
    }

    private void Init()
    {
        state = SlotState.Ready;

        AkSoundEngine.PostEvent("Play_Slot_Spwan_01", gameObject);

        for (int i = 0; i < slotLights.Length; i++)
        {
            slotLights[i].SetLight(false);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnClickCloseGame()
    {
        if (state == SlotState.Ready)
            base.OnClickCloseGame();
    }
}
