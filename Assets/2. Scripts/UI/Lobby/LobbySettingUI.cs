using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettingUI : MonoBehaviour
{
    public GameObject fadeImage;
    private Scrollbar scrollbar;

    private void Start()
    {
        scrollbar = gameObject.GetComponentInChildren<Scrollbar>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            gameObject.SetActive(false);

        if(scrollbar.value < 0)
        {
            fadeImage.SetActive(false);
        }
        else
        {
            fadeImage.SetActive(true);
        }
    }
}
