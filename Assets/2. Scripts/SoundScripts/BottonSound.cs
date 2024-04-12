using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottonSound : MonoBehaviour
{
    public static BottonSound instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Click(string NAME)
    {
        AkSoundEngine.PostEvent(NAME, gameObject);
    }
}
