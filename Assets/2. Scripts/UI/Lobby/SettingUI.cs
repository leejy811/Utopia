using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingUI : LobbySettingUI
{
    [Header("Resolution")]
    public TMP_Dropdown resDropdown;
    public TMP_Dropdown screenDropdown;

    private List<Resolution> resolutions = new List<Resolution>();
    private bool fullScreen;

    protected override void Start()
    {
        base.Start();
        InitResolution();
    }

    private void InitResolution()
    {
        foreach(Resolution res in Screen.resolutions)
        {
            if (res.refreshRate != Screen.currentResolution.refreshRate)
                continue;

            if (res.width % 16 != 0 || res.height % 9 != 0)
                continue;

            if (res.width / 16 != res.height / 9)
                continue;

            resolutions.Add(res);
        }

        resDropdown.options.Clear();

        for (int i = 0;i < resolutions.Count;i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = resolutions[i].width + " x " + resolutions[i].height;
            resDropdown.options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                resDropdown.value = i;
        }

        fullScreen = Screen.fullScreen;
        screenDropdown.value = Convert.ToInt32(fullScreen);

        resDropdown.RefreshShownValue();

        resDropdown.onValueChanged.AddListener(ChangeResolution);
        screenDropdown.onValueChanged.AddListener(ChangeScreenMode);
    }

    public void ChangeResolution(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, fullScreen);
    }

    public void ChangeScreenMode(int value)
    {
        int idx = resDropdown.value;
        fullScreen = Convert.ToBoolean(value);
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, fullScreen);
    }
}
