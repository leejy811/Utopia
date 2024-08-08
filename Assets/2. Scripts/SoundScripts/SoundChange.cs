using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundChange : MonoBehaviour
{
    public TextMeshProUGUI soundText;
    private Slider soundSlider;

    private void Start()
    {
        soundSlider = gameObject.GetComponent<Slider>();
    }
    
    public void ChangeSound()
    {
        AkSoundEngine.SetRTPCValue("OPTION", soundSlider.value * 100.0f);
        soundText.text = ((int)(soundSlider.value * 100.0f)).ToString();
    }
}
