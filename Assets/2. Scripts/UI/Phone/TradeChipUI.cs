using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeChipUI : MonoBehaviour
{
    public TextMeshProUGUI chipCostText;
    public TextMeshProUGUI possibleChipText;
    public TextMeshProUGUI tradeChipText;
    public TextMeshProUGUI tradeCostText;
    public TextMeshProUGUI errorMsgText;
    public Button tradeButton;

    protected int tradeChip;

    protected void OnEnable()
    {
        tradeChip = 0;
        errorMsgText.color -= new Color(0, 0, 0, 1);
        tradeButton.interactable = false;
        SetValue();
    }

    protected virtual void SetValue()
    {
        chipCostText.text = ChipManager.instance.CalcChipCost().ToString();
        tradeChipText.text = tradeChip.ToString();
        tradeCostText.text = (tradeChip * ChipManager.instance.CalcChipCost()).ToString();
    }

    protected void OnErrorMessage(string message)
    {
        errorMsgText.text = message;
        errorMsgText.color += new Color(0, 0, 0, 1);
        errorMsgText.DOFade(0.0f, 0.5f);
    }

    public void OnClickInitTrade()
    {
        tradeButton.interactable = false;
        tradeChip = 0;
        SetValue();
    }

    public virtual void OnClickAddTrade()
    {
        tradeButton.interactable = true;
        SetValue();
    }

    public virtual void OnClickTrade()
    {
        
    }
}
