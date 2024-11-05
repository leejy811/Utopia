using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyChipUI : TradeChipUI
{
    public TextMeshProUGUI curMoneyText;

    protected override void SetValue()
    {
        base.SetValue();

        int possibleChip = ShopManager.instance.Money / ChipManager.instance.CalcChipCost();
        possibleChipText.text = possibleChip.ToString();
        curMoneyText.text = ShopManager.instance.Money.ToString();
    }

    public override void OnClickAddTrade()
    {
        int possibleChip = ShopManager.instance.Money / ChipManager.instance.CalcChipCost();
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
        ShopManager.instance.PayMoney(ChipManager.instance.CalcChipCost() * tradeChip);
        ChipManager.instance.TradeChip(tradeChip);
    }
}
