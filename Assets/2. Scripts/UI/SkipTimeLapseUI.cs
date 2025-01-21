using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipTimeLapseUI : MonoBehaviour
{
    public Image skipImage;
    public float skipSpeed;
    public bool isSkip;

    private void Update()
    {
        if (isSkip) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            skipImage.gameObject.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            skipImage.fillAmount += skipSpeed * Time.deltaTime;
            if (skipImage.fillAmount > 0.999f)
            {
                isSkip = true;
                skipImage.gameObject.SetActive(false);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            skipImage.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        skipImage.fillAmount = 0.0f;
        isSkip = false;
        skipImage.gameObject.SetActive(false);
    }
}
