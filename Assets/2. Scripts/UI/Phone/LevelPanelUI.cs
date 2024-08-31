using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPanelUI : MonoBehaviour
{
    public GameObject[] levels;

    private void OnEnable()
    {
        SetValue();
    }
    private void SetValue()
    {
        int level = CityLevelManager.instance.levelIdx;

        for (int i = 0; i < levels.Length; i++)
        {
            bool active = level == i;
            levels[i].gameObject.SetActive(active);
        }
    }
}
