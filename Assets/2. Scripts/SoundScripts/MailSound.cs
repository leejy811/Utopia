using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailSound : MonoBehaviour
{
    public void PlayMailSound(string soundName)
    {
        AkSoundEngine.PostEvent(soundName, gameObject);
    }
}
