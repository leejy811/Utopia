using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPanelUI : PanelUI
{
    public GameObject[] levels;

    private void OnEnable()
    {
        SetValue();
    }
    private void SetValue()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            bool active = (data as LevelPanelData).level == i;
            levels[i].gameObject.SetActive(active);
        }
    }
}
