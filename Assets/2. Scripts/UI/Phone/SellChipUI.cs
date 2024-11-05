using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellChipUI : TradeChipUI
{
    public TextMeshProUGUI remainChipText;

    protected override void SetValue()
    {
        base.SetValue();

        int possibleChip = ChipManager.instance.curChip;
        possibleChipText.text = possibleChip.ToString();
        remainChipText.text = (possibleChip - tradeChip).ToString();
    }

    public override void OnClickAddTrade()
    {
        int possibleChip = ChipManager.instance.curChip;
        if (possibleChip == tradeChip)
        {
            OnErrorMessage("�� �̻� ��ȯ�� �� �����ϴ�.");
            return;
        }

        tradeChip++;
        base.OnClickAddTrade();
    }

    public override void OnClickTrade()
    {
        ShopManager.instance.GetMoney(ChipManager.instance.CalcChipCost() * tradeChip);
        ChipManager.instance.TradeChip(-tradeChip);
    }
}
