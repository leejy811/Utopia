using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradeResultUI : MonoBehaviour
{
    public TextMeshProUGUI curMoneyText;
    public TextMeshProUGUI curChipText;

    private void OnEnable()
    {
        curMoneyText.text = ShopManager.instance.Money.ToString();
        curChipText.text = ChipManager.instance.curChip.ToString();
    }
}
