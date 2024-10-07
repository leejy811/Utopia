using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipPanelUI : MonoBehaviour
{
    public GameObject[] panels;

    private void OnEnable()
    {
        OnClickBackButton();
    }

    public void OnClickBackButton()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);
    }
}