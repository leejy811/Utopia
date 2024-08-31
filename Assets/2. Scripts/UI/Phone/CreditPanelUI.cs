using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditPanelUI : MonoBehaviour
{
    public CreditUI[] credits;

    private void OnEnable()
    {
        SetValue();
    }
    private void SetValue()
    {
        ResultType result = RoutineManager.instance.weekResult;

        for (int i = 0;i < credits.Length; i++)
        {
            bool active = (int)result == i;
            credits[i].gameObject.SetActive(active);
        }
    }
}
