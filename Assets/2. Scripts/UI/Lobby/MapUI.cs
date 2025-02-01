using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using DG.Tweening.Plugins.Core.PathCore;

public class MapUI : MonoBehaviour
{
    public MapType mapType;

    [Header("Image")]
    public Button dataResetButton;
    public ScaleMouseOver playButton;
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
            SetMapImage(!GameManager.instance.isLoad && mapType == MapType.Utopia);
        else
            SetMapImage(true);

        SetStat();

        dataResetButton.gameObject.SetActive(!GameManager.instance.isLoad && DataBaseManager.instance.clearData[(int)mapType].playTime != 0.0f);
    }

    private void SetStat()
    {
        if (!GameManager.instance.isLoad)
        {
            string path = Application.persistentDataPath + "/ClearData_" + mapType.ToString();
            if (!File.Exists(path))
            {
                SetZeroStat();
                return;
            }
        }


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

    private void SetZeroStat()
    {
        dayText.text = "0000 / 00 / 00";
        timeText.text = "0시간 0분";
        destroyText.text = "0";
        happinessText.text = "0";

        buildingText.text = "";
        for (int i = 0; i < System.Enum.GetValues(typeof(BuildingType)).Length; i++)
        {
            buildingText.text += "<color=#" + colorStr[i] + ">" + "0" + "</color>";
            if (i != System.Enum.GetValues(typeof(BuildingType)).Length - 1)
                buildingText.text += " / ";
        }
    }

    private void SetMapImage(bool isExist)
    {
        mapImage.gameObject.SetActive(isExist);
        lockImage.gameObject.SetActive(!isExist);
        playButton.interactable = isExist;
        playButton.GetComponent<Button>().interactable = isExist;
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " 시간 " + ((int)minute % 60).ToString() + " 분";

        return res;
    }

    public void OnClickResetData()
    {
        SetZeroStat();
        dataResetButton.gameObject.SetActive(false);
        DataBaseManager.instance.ResetClearData(mapType, "/ClearData_");
    }
}
