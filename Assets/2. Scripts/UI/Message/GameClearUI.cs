using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClearUI : UIElement
{
    [Header("Date")]
    public TextMeshProUGUI curDayText;
    public TextMeshProUGUI playTimeText;

    [Header("Credit")]
    public CreditScoreBar creditScore;

    [Header("Building")]
    public TextMeshProUGUI[] buildingCntText;
    public TextMeshProUGUI removeBuildingText;

    private void SetValue()
    {
        curDayText.text = RoutineManager.instance.day.ToString("yyyy년 MM월 dd일");
        playTimeText.text = SecondToString(RoutineManager.instance.playTime);

        int score = RoutineManager.instance.creditRating;
        creditScore.SetScore(score, score);

        int[,] count = BuildingSpawner.instance.buildingGradeCount;
        for (int i = 0;i < buildingCntText.Length;i++)
        {
            buildingCntText[i].text = count[i, 0].ToString() + " / " + count[i, 1].ToString() + " / " + count[i, 2].ToString();
        }

        removeBuildingText.text = BuildingSpawner.instance.buildingRemoveCount.ToString();
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " 시간 " + ((int)minute).ToString() + " 분";

        return res;
    }

    private void OnEnable()
    {
        SetValue();
    }
}
