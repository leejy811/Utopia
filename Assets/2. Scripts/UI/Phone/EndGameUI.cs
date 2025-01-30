using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndGameUI : MonoBehaviour
{
    public EventState state;

    [Header("Image Transform")]
    public Transform backGroundImage;
    public Transform frontImage;
    public Transform buttons;
    public Transform inline;
    public Transform saveInfo;

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
    public Button exitButton;

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
    }

    private string SecondToString(float second)
    {
        float minute = second / 60.0f;
        float hour = minute / 60.0f;

        string res = ((int)hour).ToString() + " 시간 " + ((int)minute % 60).ToString() + " 분";

        return res;
    }

    private void InitAnimation()
    {
        backGroundImage.DOLocalMove(Vector3.zero, 1f);
        frontImage.DOLocalMove(Vector3.zero, 1f);
        buttons.DOLocalMoveY(buttons.transform.localPosition.y + 130.0f, 1f).OnComplete(() =>
        {
            Image[] images = frontImage.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = frontImage.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (Image image in images)
            {
                image.DOFade(1.0f, 0.7f);
            }

            foreach (TextMeshProUGUI text in texts)
            {
                text.DOFade(1.0f, 0.7f);
            }

            inline.DOLocalMoveX(inline.transform.localPosition.x + 10.0f, 0.7f);
        });
    }

    private void SetData()
    {
        MapType type = GameManager.instance.curMapType;
        DataBaseManager.instance.DeleteMapData(type, "/MapData_");

        if (state == EventState.GameClear)
        {
            if (!File.Exists(Application.persistentDataPath + "/ClearData_" + GameManager.instance.curMapType.ToString()))
            {
                exitButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        SetValue();
        SetData();
        InitAnimation();
    }

    public void OnClickSaveData()
    {
        saveInfo.DOLocalMoveY(saveInfo.transform.localPosition.y - 40.0f, 0.7f);
        saveInfo.gameObject.GetComponent<Image>().DOFade(1.0f, 0.7f).OnComplete(() =>
        {
            saveInfo.gameObject.GetComponent<Image>().DOFade(0.0f, 1.0f);
        });
        saveButton.interactable = false;
        if (!exitButton.gameObject.activeSelf) exitButton.gameObject.SetActive(true);

        MapType type = GameManager.instance.curMapType;
        DataBaseManager.instance.SaveMapData(DataBaseManager.instance.clearData[(int)type], type, "/ClearData_");
    }
}
