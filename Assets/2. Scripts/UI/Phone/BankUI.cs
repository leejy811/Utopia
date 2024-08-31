using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankUI : MonoBehaviour
{
    public RequestPayUI[] RequestPays;

    private PhoneUI phone;

    private void Start()
    {
        phone = gameObject.GetComponentInParent<PhoneUI>();
    }

    private void OnEnable()
    {
        if (RoutineManager.instance.isPay)
            phone.ChaneState(PhoneState.Credit);
        else
            SetRequestValue();
    }

    private void SetRequestValue()
    {
        int debt = RoutineManager.instance.debt;
        bool isPay = RoutineManager.instance.weekResult != ResultType.PayFail;
        int idx = Convert.ToInt32(isPay);

        RequestPays[idx].gameObject.SetActive(true);
        RequestPays[(idx + 1) % 2].gameObject.SetActive(false);
    }
}
