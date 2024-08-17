using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaybackUI : ReceiptUI
{
    public TextMeshProUGUI paybackText;

    protected override void SetValue()
    {
        base.SetValue();

        int paybackMoney = (int)(RoutineManager.instance.debt * 0.2f);
        paybackText.text = paybackMoney.ToString("C");
    }
}
