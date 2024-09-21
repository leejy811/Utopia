using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("Button")]
    public Button[] buttons;
    public Image[] buttonImages;

    [Header("Text")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI destroyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI buildingText;

    [Header("Lock")]
    public bool isLock;

    private string[] colorStr = { "75CD1A", "FFC91A", "FD4D91", "9461FF" };

    private void OnEnable()
    {
        if (isLock) return;

        buttons[0].onClick.Invoke();
        SetStat();
    }

    public void OnClickButton(int idx)
    {
        int otherIdx = idx == 0 ? 1 : 0;

        buttonImages[otherIdx].color = Color.white * 0.66666f;
        buttonImages[idx].color = Color.white * 1.0f;
    }

    public void SetStat()
    {
        if (PlayerPrefs.GetInt("isPlay", 0) == 0)
            return;

        dayText.text = PlayerPrefs.GetString("ClearDay");
        timeText.text = PlayerPrefs.GetString("PlayTime");
        destroyText.text = PlayerPrefs.GetString("DestroyCount");
        happinessText.text = PlayerPrefs.GetString("Happiness");

        buildingText.text = "";
        for (int i = 0; i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            buildingText.text += "<color=#" + colorStr[i] + ">" + PlayerPrefs.GetInt((BuildingType)i + " Building Count") + "</color>";
            if (i != System.Enum.GetValues(typeof(BuildingType)).Length - 1)
                buildingText.text += " / ";
        }
    }
}
