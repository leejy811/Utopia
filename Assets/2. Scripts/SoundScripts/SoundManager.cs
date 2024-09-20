using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public enum SoundType { BGM, SFX }

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public float initialVolume;
    public float[] volumes;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitVolume(initialVolume);
    }

    private void InitVolume(float volume)
    {
        volumes = new float[2];
        ChangeVolume(SoundType.BGM, volume, "BGM_VOL");
        ChangeVolume(SoundType.SFX, volume, "SFX_VOL");
    }

    public void ChangeVolume(SoundType type, float volume, string valueName)
    {
        SetValue(valueName, volume);
        volumes[(int)type] = volume;
    }

    public void PostEvent(string name)
    {
        AkSoundEngine.PostEvent(name, gameObject);
    }

    public void SetValue(string name, float value)
    {
        AkSoundEngine.SetRTPCValue(name, value);
    }

    public void SetBGM(string sceneName)
    {
        StartCoroutine(DelaySetBGM(sceneName));
    }

    IEnumerator DelaySetBGM(string sceneName)
    {
        if (sceneName == "LobbyScene")
            SetIngameBGM(false);

        yield return new WaitForSeconds(0.2f);

        if (sceneName == "InGameScene")
            SetIngameBGM(true);
    }

    public void SetIngameBGM(bool isPlay)
    {
        string state = isPlay ? "Play_" : "Stop_";

        PostEvent(state + "BGM_01");

        PostEvent(state + "APT_event_DESOLATE_01");
        PostEvent(state + "ForestAmbience");
        PostEvent(state + "testing");
        PostEvent(state + "Wind");
        PostEvent(state + "protest_002");
        PostEvent(state + "APT_event_DISEASE_01");

        PostEvent(state + "APT_event_FIRE_01");
        PostEvent(state + "APT_event_THIEF_01");
        PostEvent(state + "SP_Decrease_price_01");

        PostEvent(state + "SP_Discount_01");
        PostEvent(state + "SP_Festival_01");
        PostEvent(state + "SP_Few_jobs_01");

        PostEvent(state + "SP_Increase_price_01");
        PostEvent(state + "SP_Low_in_stock_01");
        PostEvent(state + "SP_Many_jobs_01");

        PostEvent(state + "NAPT_event_DESOLATE_01");
        PostEvent(state + "NAPT_event_DISEASE_01");
        PostEvent(state + "NAPT_event_FIRE_01");

        PostEvent(state + "NAPT_event_THIEF_01");
        PostEvent(state + "NSP_Decrease_price_01");
        PostEvent(state + "NSP_Discount_01");
        PostEvent(state + "NSP_Festival_01");

        PostEvent(state + "NSP_Few_jobs_01");
        PostEvent(state + "NSP_Increase_price_01");
        PostEvent(state + "NSP_Low_in_stock_01");
        PostEvent(state + "NSP_Many_jobs_01");
        PostEvent(state + "1APT_event_DESOLATE_01");
        PostEvent(state + "1APT_event_DISEASE_01");

        PostEvent(state + "1APT_event_FIRE_01");
        PostEvent(state + "1APT_event_THIEF_01");
        PostEvent(state + "1SP_Decrease_price_01");
        PostEvent(state + "1SP_Discount_01");
        PostEvent(state + "1SP_Festival_01");
        PostEvent(state + "1SP_Few_jobs_01");

        PostEvent(state + "1SP_Increase_price_01");
        PostEvent(state + "1SP_Low_in_stock_01");
        PostEvent(state + "1SP_Many_jobs_01");
        PostEvent(state + "NNAPT_event_DESOLATE_01");
        PostEvent(state + "NNAPT_event_DISEASE_01");
        PostEvent(state + "NNAPT_event_FIRE_01");

        PostEvent(state + "NNAPT_event_THIEF_01");
        PostEvent(state + "NNSP_Decrease_price_01");
        PostEvent(state + "NNSP_Discount_01");
        PostEvent(state + "NNSP_Festival_01");
        PostEvent(state + "NNSP_Few_jobs_01");
        PostEvent(state + "NNSP_Increase_price_01");

        PostEvent(state + "NNSP_Low_in_stock_01");
        PostEvent(state + "NNSP_Many_jobs_01");

        if (isPlay)
            AkSoundEngine.SetState("BGM", "BASIC");
    }
}
