using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingUI : MonoBehaviour
{
    [Header("Resolution")]
    public TMP_Dropdown resDropdown;
    public TMP_Dropdown screenDropdown;

    [Header("Sound")]
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private List<Resolution> resolutions = new List<Resolution>();
    private FullScreenMode fullScreen;

    private void Start()
    {
        InitResolution();
        InitVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            gameObject.SetActive(false);
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

        fullScreen = Screen.fullScreenMode;
        screenDropdown.value = Convert.ToInt32(fullScreen);

        resDropdown.RefreshShownValue();

        resDropdown.onValueChanged.AddListener(ChangeResolution);
        screenDropdown.onValueChanged.AddListener(ChangeScreenMode);
    }

    private void InitVolume()
    {
        float[] volumes = SoundManager.instance.volumes;
        bgmText.text = ((int)volumes[(int)SoundType.BGM]).ToString();
        sfxText.text = ((int)volumes[(int)SoundType.SFX]).ToString();

        bgmSlider.value = volumes[(int)SoundType.BGM] / 100.0f;
        sfxSlider.value = volumes[(int)SoundType.SFX] / 100.0f;
    }

    public void ChangeResolution(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, fullScreen);
    }

    public void ChangeScreenMode(int value)
    {
        int idx = resDropdown.value;
        fullScreen = Convert.ToBoolean(value) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, fullScreen);
    }

    public void ChangeBGMVolume(float value)
    {
        SoundManager.instance.ChangeVolume(SoundType.BGM, value * 100.0f, "BGM_VOL");
        bgmText.text = ((int)(value * 100.0f)).ToString();
    }

    public void ChangeSFXVolume(float value)
    {
        SoundManager.instance.ChangeVolume(SoundType.SFX, value * 100.0f, "SFX_VOL");
        sfxText.text = ((int)(value * 100.0f)).ToString();
    }
}
