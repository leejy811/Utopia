using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    public EventState state;

    [Header("Date")]
    public TextMeshProUGUI curDayText;
    public TextMeshProUGUI playTimeText;

    [Header("Credit")]
    public TextMeshProUGUI creditScoreText;

    [Header("Building")]
    public TextMeshProUGUI[] buildingCntText;
    public TextMeshProUGUI removeBuildingText;
    public TextMeshProUGUI happinessText;

    [Header("Button")]
    public Button saveButton;

    private void SetValue()
    {
        DateTime day = RoutineManager.instance.day;
        curDayText.text = day.ToString("yy") + "/" + day.ToString("MM") + "/" + day.ToString("dd");
        playTimeText.text = SecondToString(RoutineManager.instance.playTime);

        int score = RoutineManager.instance.creditRating;
        int endScore = state == EventState.GameOver ? 0 : score;
        creditScoreText.text = endScore.ToString() + "<size=10>점</size>";

        int[,] count = BuildingSpawner.instance.buildingGradeCount;
        for (int i = 0;i < buildingCntText.Length;i++)
        {
            buildingCntText[i].text = count[i, 0].ToString() + " / " + count[i, 1].ToString() + " / " + count[i, 2].ToString();
        }

        removeBuildingText.text = BuildingSpawner.instance.buildingRemoveCount.ToString();

        int happiness = (int)RoutineManager.instance.cityHappiness;
        happinessText.text = happiness.ToString();

        if (state == EventState.GameClear)
        {
            if (!File.Exists(Application.persistentDataPath + "/ClearData_" + GameManager.instance.curMapType.ToString()))
            {
                OnClickSaveData();
            }
        }
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " 시간 " + ((int)minute % 60).ToString() + " 분";

        return res;
    }

    private void OnEnable()
    {
        SetValue();
    }

    public void OnClickSaveData()
    {
        saveButton.interactable = false;

        MapType type = GameManager.instance.curMapType;
        DataBaseManager.instance.SaveMapData(DataBaseManager.instance.clearData[(int)type], type, "/ClearData_");
        DataBaseManager.instance.DeleteMapData(type, "/MapData_");
    }
}
