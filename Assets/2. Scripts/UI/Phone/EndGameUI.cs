using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public EventState state;

    [Header("Date")]
    public TextMeshProUGUI curDayText;
    public TextMeshProUGUI playTimeText;

    [Header("Credit")]
    public CreditScoreBar creditScore;

    [Header("Building")]
    public TextMeshProUGUI[] buildingCntText;
    public TextMeshProUGUI removeBuildingText;
    public TextMeshProUGUI happinessText;

    private void SetValue()
    {
        DateTime day = RoutineManager.instance.day;
        curDayText.text = day.ToString("yy") + "/" + day.ToString("MM") + "/" + day.ToString("dd");
        playTimeText.text = SecondToString(RoutineManager.instance.playTime);

        int score = RoutineManager.instance.creditRating;
        int endScore = state == EventState.GameOver ? 0 : score;
        creditScore.SetScore(score, endScore);

        int[,] count = BuildingSpawner.instance.buildingGradeCount;
        for (int i = 0;i < buildingCntText.Length;i++)
        {
            buildingCntText[i].text = count[i, 0].ToString() + " / " + count[i, 1].ToString() + " / " + count[i, 2].ToString();
        }

        removeBuildingText.text = BuildingSpawner.instance.buildingRemoveCount.ToString();

        int happiness = (int)RoutineManager.instance.cityHappiness;
        happinessText.text = happiness.ToString();

        if (PlayerPrefs.GetInt("isPlay", 0) == 0)
        {
            PlayerPrefs.SetInt("isPlay", 1);
            OnClickSaveData();
        }
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " ½Ã°£ " + ((int)minute).ToString() + " ºÐ";

        return res;
    }

    private void OnEnable()
    {
        SetValue();
    }

    public void OnClickSaveData()
    {
        PlayerPrefs.SetString("PlayTime", playTimeText.text);
        PlayerPrefs.SetString("ClearDay", curDayText.text);

        for (int i = 0; i < buildingCntText.Length; i++)
        {
            PlayerPrefs.SetString((BuildingType)i + " Building Count", buildingCntText[i].text);
        }

        PlayerPrefs.SetString("DestroyCount", removeBuildingText.text);
        PlayerPrefs.SetString("Happiness", happinessText.text);

        Debug.Log("Save");
    }
}
