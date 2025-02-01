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
        InitVolume();
    }

    private void InitVolume()
    {
        volumes = DataBaseManager.instance.LoadSoundData();
        ChangeVolume(SoundType.BGM, volumes[(int)SoundType.BGM], "BGM_VOL");
        ChangeVolume(SoundType.SFX, volumes[(int)SoundType.SFX], "SFX_VOL");
    }

    public void ChangeVolume(SoundType type, float volume, string valueName)
    {
        SetValue(valueName, volume);
        volumes[(int)type] = volume;
        DataBaseManager.instance.SaveSoundData(volumes);
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

        if (sceneName != "LobbyScene")
            SetIngameBGM(true);
    }

    public void SetIngameBGM(bool isPlay)
    {
        string state = isPlay ? "Play_" : "Stop_";

        if(GameManager.instance.curMapType == MapType.Utopia)
        {
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
        }
        else if (GameManager.instance.curMapType == MapType.Totopia)
        {
            PostEvent(state + "TOBGM");
            PostEvent(state + "LOOP_bar");
            PostEvent(state + "LOOP_vomit_01");
            PostEvent(state + "Wind");

            PostEvent(state + "TO_Decrease_price_01");
            PostEvent(state + "TO_Decrease_price_02");
            PostEvent(state + "TO_Decrease_price_03");
            PostEvent(state + "TO_Decrease_price_04");

            PostEvent(state + "TO_Discount_01");
            PostEvent(state + "TO_Discount_02");
            PostEvent(state + "TO_Discount_03");
            PostEvent(state + "TO_Discount_04");

            PostEvent(state + "TO_event_DESOLATE_01");
            PostEvent(state + "TO_event_DESOLATE_02");
            PostEvent(state + "TO_event_DESOLATE_03");
            PostEvent(state + "TO_event_DESOLATE_04");

            PostEvent(state + "TO_event_DISEASE_01");
            PostEvent(state + "TO_event_DISEASE_02");
            PostEvent(state + "TO_event_DISEASE_03");
            PostEvent(state + "TO_event_DISEASE_04");

            PostEvent(state + "TO_event_FIRE_01");
            PostEvent(state + "TO_event_FIRE_02");
            PostEvent(state + "TO_event_FIRE_03");
            PostEvent(state + "TO_event_FIRE_04");

            PostEvent(state + "TO_event_THIEF_01");
            PostEvent(state + "TO_event_THIEF_02");
            PostEvent(state + "TO_event_THIEF_03");
            PostEvent(state + "TO_event_THIEF_04");

            PostEvent(state + "TO_Festival_01");
            PostEvent(state + "TO_Festival_02");
            PostEvent(state + "TO_Festival_03");
            PostEvent(state + "TO_Festival_04");

            PostEvent(state + "TO_Few_jobs_01");
            PostEvent(state + "TO_Few_jobs_02");
            PostEvent(state + "TO_Few_jobs_03");
            PostEvent(state + "TO_Few_jobs_04");

            PostEvent(state + "TO_Increase_price_01");
            PostEvent(state + "TO_Increase_price_02");
            PostEvent(state + "TO_Increase_price_03");
            PostEvent(state + "TO_Increase_price_04");

            PostEvent(state + "TO_Low_in_stock_01");
            PostEvent(state + "TO_Low_in_stock_02");
            PostEvent(state + "TO_Low_in_stock_03");
            PostEvent(state + "TO_Low_in_stock_04");

            PostEvent(state + "TO_Many_jobs_01");
            PostEvent(state + "TO_Many_jobs_02");
            PostEvent(state + "TO_Many_jobs_03");
            PostEvent(state + "TO_Many_jobs_04");
        }

        if (isPlay)
            RoutineManager.instance.SetCreditRating(0);
    }
}
