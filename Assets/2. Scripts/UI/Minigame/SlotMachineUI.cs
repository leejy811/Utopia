using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SlotType { Spade, Heart, Clover, Dia, Joker }

public class SlotMachineUI : MinigameUI
{
    public TextMeshProUGUI payChipText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI playTimesText;

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        SetValue();
    }

    public override void SetValue()
    {
        payChipText.text = curGameBuilding.betChip.ToString();
        curChipText.text = ChipManager.instance.curChip.ToString();
        playTimesText.text = curGameBuilding.betTimes.ToString();
    }
}
