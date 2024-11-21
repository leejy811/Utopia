using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class MapUI : MonoBehaviour
{
    public MapType mapType;

    [Header("Image")]
    public Image mapImage;
    public Image lockImage;

    [Header("Text")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI destroyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI buildingText;

    private string[] colorStr = { "75CD1A", "FFC91A", "FD4D91", "9461FF" };

    private void OnEnable()
    {
        string path = "";

        if (GameManager.instance.isLoad)
            path = Application.persistentDataPath + "/MapData_" + mapType.ToString();
        else
            path = Application.persistentDataPath + "/ClearData_" + (mapType - 1).ToString();

        if (!File.Exists(path))
        {
            if(!GameManager.instance.isLoad && (mapType == MapType.Utopia || mapType == MapType.Totopia))
            {
                SetMapImage(true);
            }
            else
            {
                SetMapImage(false);
            }
            return;
        }

        SetMapImage(true);

        SetStat();
    }

    public void SetStat()
    {
        MapData data = new MapData();

        if (GameManager.instance.isLoad)
            data = DataBaseManager.instance.mapData[(int)mapType];
        else
            data = DataBaseManager.instance.clearData[(int)mapType];

        dayText.text = data.day.year.ToString("0000") + " / "+ data.day.month.ToString("00") + " / " + data.day.day.ToString("00");
        timeText.text = SecondToString(data.playTime);
        destroyText.text = data.destroyCount.ToString();
        happinessText.text = data.happiness.ToString();

        buildingText.text = "";
        for (int i = 0; i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            buildingText.text += "<color=#" + colorStr[i] + ">" + data.buildingTypeCount[i] + "</color>";
            if (i != System.Enum.GetValues(typeof(BuildingType)).Length - 1)
                buildingText.text += " / ";
        }
    }

    private void SetMapImage(bool isExist)
    {
        mapImage.gameObject.SetActive(isExist);
        lockImage.gameObject.SetActive(!isExist);
        gameObject.GetComponent<ScaleMouseOver>().interactable = isExist;
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " ½Ã°£ " + ((int)minute % 60).ToString() + " ºÐ";

        return res;
    }
}
