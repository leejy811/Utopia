using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MCSS : MonoBehaviour
{
    bool isConstruction = false;
    // Start is called before the first frame update
    void Start()
    {

        AkSoundEngine.PostEvent("Play_BGM_01", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Play_ForestAmbience", gameObject);
        AkSoundEngine.PostEvent("Play_testing", gameObject);
        AkSoundEngine.PostEvent("Play_Wind", gameObject);
        AkSoundEngine.PostEvent("Play_protest_002", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DISEASE_01", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_FIRE_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Play_SP_Decrease_price_01", gameObject);

        AkSoundEngine.PostEvent("Play_SP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Play_SP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Play_SP_Few_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Play_SP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_SP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Play_SP_Many_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Play_NAPT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Play_NAPT_event_DISEASE_01", gameObject);
        AkSoundEngine.PostEvent("Play_NAPT_event_FIRE_01", gameObject);

        AkSoundEngine.PostEvent("Play_NAPT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Festival_01", gameObject);

        AkSoundEngine.PostEvent("Play_NSP_Few_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Play_NSP_Many_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Play_1APT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Play_1APT_event_DISEASE_01", gameObject);

        AkSoundEngine.PostEvent("Play_1APT_event_FIRE_01", gameObject);
        AkSoundEngine.PostEvent("Play_1APT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Few_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Play_1SP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Play_1SP_Many_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNAPT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNAPT_event_DISEASE_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNAPT_event_FIRE_01", gameObject);

        AkSoundEngine.PostEvent("Play_NNAPT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Few_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Increase_price_01", gameObject);

        AkSoundEngine.PostEvent("Play_NNSP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Play_NNSP_Many_jobs_01", gameObject);

        AkSoundEngine.SetState("BGM", "BASIC");
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingSpawner.instance.buildings.Count >= 1 && isConstruction)
        {
            isConstruction = false;
            AkSoundEngine.PostEvent("Stop_construction", gameObject);
        } 
        else if (BuildingSpawner.instance.buildings.Count < 1&& !isConstruction)
        {
            isConstruction = true;
            AkSoundEngine.PostEvent("Play_construction", gameObject);
        }
    }

    public void StopInGameBGM()
    {
        AkSoundEngine.PostEvent("Stop_BGM_01", gameObject);

        AkSoundEngine.PostEvent("Stop_APT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_ForestAmbience", gameObject);
        AkSoundEngine.PostEvent("Stop_testing", gameObject);
        AkSoundEngine.PostEvent("Stop_Wind", gameObject);
        AkSoundEngine.PostEvent("Stop_protest_002", gameObject);
        AkSoundEngine.PostEvent("Stop_APT_event_DISEASE_01", gameObject);

        AkSoundEngine.PostEvent("Stop_APT_event_FIRE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_APT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Stop_SP_Decrease_price_01", gameObject);

        AkSoundEngine.PostEvent("Stop_SP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Stop_SP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Stop_SP_Few_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Stop_SP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_SP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Stop_SP_Many_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Stop_NAPT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NAPT_event_DISEASE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NAPT_event_FIRE_01", gameObject);

        AkSoundEngine.PostEvent("Stop_NAPT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Festival_01", gameObject);

        AkSoundEngine.PostEvent("Stop_NSP_Few_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NSP_Many_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1APT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1APT_event_DISEASE_01", gameObject);

        AkSoundEngine.PostEvent("Stop_1APT_event_FIRE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1APT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Few_jobs_01", gameObject);

        AkSoundEngine.PostEvent("Stop_1SP_Increase_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Stop_1SP_Many_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNAPT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNAPT_event_DISEASE_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNAPT_event_FIRE_01", gameObject);

        AkSoundEngine.PostEvent("Stop_NNAPT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Decrease_price_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Discount_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Festival_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Few_jobs_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Increase_price_01", gameObject);

        AkSoundEngine.PostEvent("Stop_NNSP_Low_in_stock_01", gameObject);
        AkSoundEngine.PostEvent("Stop_NNSP_Many_jobs_01", gameObject);
    }
}



