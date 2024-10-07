using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradeChipUI : MonoBehaviour
{
    public TextMeshProUGUI chipCostText;
    public TextMeshProUGUI possibleChipText;
    public TextMeshProUGUI tradeChipText;
    public TextMeshProUGUI tradeCostText;
    public TextMeshProUGUI errorMsgText;

    protected int tradeChip;

    protected void OnEnable()
    {
        tradeChip = 0;
        errorMsgText.color -= new Color(0, 0, 0, 1);
        SetValue();
    }

    protected virtual void SetValue()
    {
        chipCostText.text = ChipManager.instance.CalcChipCost().ToString();
        tradeChipText.text = tradeChip.ToString();
        tradeCostText.text = (tradeChip * ChipManager.instance.CalcChipCost()).ToString();
    }

    protected void OnErrorMessage()
    {
        errorMsgText.color += new Color(0, 0, 0, 1);
        errorMsgText.DOFade(0.0f, 0.5f);
    }

    public void OnClickInitTrade()
    {
        tradeChip = 0;
        SetValue();
    }

    public virtual void OnClickAddTrade()
    {
        SetValue();
    }

    public virtual void OnClickTrade()
    {
        
    }
}
