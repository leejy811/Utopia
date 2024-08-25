using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCSS : MonoBehaviour
{
    bool isConstruction = false;
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Play_BGMsequnce1", gameObject);
        AkSoundEngine.PostEvent("Play_BGM4", gameObject);
        AkSoundEngine.PostEvent("Play_ForestAmbience", gameObject);
        AkSoundEngine.PostEvent("Play_testing", gameObject);
        AkSoundEngine.PostEvent("Play_Wind", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_scary_2_noHeel_v1", gameObject);
        AkSoundEngine.PostEvent("Play_protest_002", gameObject);
        AkSoundEngine.PostEvent("Play_BGM5", gameObject);

        AkSoundEngine.PostEvent("Play_ATP_event_water_outagev1", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_water_outagev2", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_water_outagev3", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_water_outagev4", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_BLACKOUT_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DESOLATE_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DISEASE_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_FIRE_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_NOISY_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_SEWERSMELL_001", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_THIEF_001", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_ELEVATOR_001_v2", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_scary_2_noHeel_v2", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_BLACKOUT_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DESOLATE_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DISEASE_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_FIRE_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_NOISY_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_SEWERSMELL_01", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_THIEF_01", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_ELEVATOR_01_v2", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_scary_2_noHeel_v3", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_BLACKOUT_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DESOLATE_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DISEASE_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_FIRE_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_NOISY_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_SEWERSMELL_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_THIEF_01_1", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_ELEVATOR_01_v2_1", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_scary_2_noHeel_v4", gameObject);

        AkSoundEngine.PostEvent("Play_APT_event_BLACKOUT_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DESOLATE_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_DISEASE_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_FIRE_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_NOISY_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_SEWERSMELL_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_APT_event_THIEF_001_1", gameObject);
        AkSoundEngine.PostEvent("Play_ATP_event_ELEVATOR_001_v2_1", gameObject);


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
        AkSoundEngine.PostEvent("Stop", gameObject);
        AkSoundEngine.PostEvent("Stop_01", gameObject);
        AkSoundEngine.PostEvent("Stop_02", gameObject);
        AkSoundEngine.PostEvent("Stop_03", gameObject);
        AkSoundEngine.PostEvent("Stop_04", gameObject);
    }




}



