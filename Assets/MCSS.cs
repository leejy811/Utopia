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
        AkSoundEngine.PostEvent("Play_ForestAmbience", gameObject);
        AkSoundEngine.PostEvent("Play_testing", gameObject);
        AkSoundEngine.PostEvent("Play_Wind", gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingSpawner.instance.buildings.Count >= 3 && isConstruction)
        {
            isConstruction = false;
            AkSoundEngine.PostEvent("Stop_construction", gameObject);
        } 
        else if (BuildingSpawner.instance.buildings.Count < 3 && !isConstruction)
        {
            isConstruction = true;
            AkSoundEngine.PostEvent("Play_construction", gameObject);
        }
    }
}
