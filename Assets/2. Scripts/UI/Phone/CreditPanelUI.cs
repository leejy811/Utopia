using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditPanelUI : PanelUI
{
    public CreditUI[] credits;

    private CreditPanelData creditData;

    private void OnEnable()
    {
        SetValue();
    }
    private void SetValue()
    {
        creditData = data as CreditPanelData;
        ResultType result = creditData.result;

        for (int i = 0;i < credits.Length; i++)
        {
            bool active = (int)result == i;
            credits[i].data = creditData;
            credits[i].gameObject.SetActive(active);
        }
    }
}
