using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIElement
{
    public GameObject fadeImage;

    [Header("Sound")]
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    [Header("Resolution")]
    public Dropdown resDropdown;
    public Dropdown screenDropdown;
    private List<Resolution> resolutions = new List<Resolution>();
    private FullScreenMode screenMode;
    private int[] screenModeIdx = { 1, 0, 3 };

    private void Start()
    {
        InitResolution();
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
            resolutions.Add(res);
        }

        resDropdown.options.Clear();
        screenMode = FullScreenMode.FullScreenWindow;

        for(int i = 0;i < resolutions.Count;i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
            resDropdown.options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                resDropdown.value = i;
        }

        resDropdown.RefreshShownValue();
    }

    public void ChangeResolution(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, screenMode, resolutions[value].refreshRate);
    }

    public void ChangeScreenMode(int value)
    {
        int idx = resDropdown.value;
        screenMode = (FullScreenMode)screenModeIdx[value];
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, screenMode, resolutions[idx].refreshRate);
    }

    public void Scroll(float value)
    {
        if (value < 0)
        {
            fadeImage.SetActive(false);
        }
        else
        {
            fadeImage.SetActive(true);
        }
    }

    public void ChangeBGMSound(float value)
    {
        AkSoundEngine.SetRTPCValue("OPTION", value * 100.0f);
        bgmText.text = ((int)(value * 100.0f)).ToString();
    }

    public void ChangeSFXSound(float value)
    {
        AkSoundEngine.SetRTPCValue("OPTION", value * 100.0f);
        sfxText.text = ((int)(value * 100.0f)).ToString();
    }
}
