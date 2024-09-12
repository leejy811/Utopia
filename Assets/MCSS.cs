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
    private void OnDestroy()
    {
        //AkSoundEngine.PostEvent("Stop", gameObject);
        //AkSoundEngine.PostEvent("Stop_01", gameObject);
        //AkSoundEngine.PostEvent("Stop_02", gameObject);
        //AkSoundEngine.PostEvent("Stop_03", gameObject);
        //AkSoundEngine.PostEvent("Stop_04", gameObject);
    }

    public void StopInGameBGM()
    {
        //AkSoundEngine.PostEvent("Stop_BGMsequnce1", gameObject);
        //AkSoundEngine.PostEvent("Stop_BGM4", gameObject);
        AkSoundEngine.PostEvent("Stop_ForestAmbience", gameObject);
        AkSoundEngine.PostEvent("Stop_testing", gameObject);
        AkSoundEngine.PostEvent("Stop_Wind", gameObject);
        AkSoundEngine.PostEvent("Stop_protest_002", gameObject);
        //AkSoundEngine.PostEvent("Stop_BGM5", gameObject);

        //AkSoundEngine.PostEvent("Stop_APT_event_DISEASE_001", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_FIRE_001", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_THIEF_001", gameObject);

        //AkSoundEngine.PostEvent("Stop_APT_event_DISEASE_01", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_FIRE_01", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_THIEF_01", gameObject);

        //AkSoundEngine.PostEvent("Stop_APT_event_DISEASE_01_1", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_FIRE_01_1", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_THIEF_01_1", gameObject);

        //AkSoundEngine.PostEvent("Stop_APT_event_DISEASE_001_1", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_FIRE_001_1", gameObject);
        //AkSoundEngine.PostEvent("Stop_APT_event_THIEF_001_1", gameObject);
    }
}



