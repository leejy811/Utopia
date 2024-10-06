using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SolutionUIInfo
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI probText;
}

[Serializable]
public struct ValueSlider
{
    public TextMeshProUGUI minText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI curText;
    public TextMeshProUGUI stringText;
    public Slider slider;
}

public class BuildingIntroUI : MonoBehaviour
{
    [Header("Building")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingInfoText;
    public TextMeshProUGUI happinessText;

    [Header("Event")]
    public BuildingEventUIInfo[] eventUIInfos;
    public TextMeshProUGUI[] eventTexts;

    [Header("Slider Info")]
    public string[] boundaryString;
    public Color[] sliderColor;

    public string[] typeString = { "�ְ�", "���", "��ȭ", "����" };
    string[] subTypeString = { "����Ʈ", "��ȭ", "��ȭ", "����", "����", "�̼�", "�ҹ�", "����", "�Ƿ�", "����" };

    public virtual void SetValue(Building building)
    {
        SetInitState();

        buildingNameText.text = building.buildingName + building.count;
        buildingInfoText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType];
        happinessText.text = building.happinessRate + "(" + GetSignString(building.happinessDifference, "+") + ")%";

        for (int i = 0;i < building.curEvents.Count; i++)
        {
            int idx = building.curEvents[i].type == EventType.Event ? 2 : 0;
            eventUIInfos[idx + i].gameObject.SetActive(true);
            eventUIInfos[idx + i].SetEventUIInfo(building.curEvents[i]);

            eventTexts[i].gameObject.SetActive(true);
            eventTexts[i].text = "��ȸ���� �߻����� ���� " + building.curEvents[i].GetEventToString();
        }

        float id1 = building.curEvents.Count == 0 ? -1f : building.curEvents[0].eventIndex + 0.5f;
        float id2 = building.curEvents.Count == 2 ? building.curEvents[1].eventIndex + 0.5f : -1f;

        AkSoundEngine.SetRTPCValue("INDEX1", id1);
        AkSoundEngine.SetRTPCValue("INDEX2", id2);

        if (building.GetEventProblemCount() == 2)
            eventTexts[1].gameObject.SetActive(false);
    }

    public void OnUI(Building building)
    {
        SetValue(building);
    }

    protected string GetSignString(int data, string zeroSign)
    {

        if (data > 0)
            return "+ " + data.ToString();
        else if (data < 0)
            return "- " + Mathf.Abs(data).ToString();
        else
            return zeroSign + " " + data.ToString();
    }

    protected void SetSlider(ValueSlider slider, BoundaryValue value, int idx)
    {
        slider.minText.text = ((int)value.min).ToString();
        slider.maxText.text = ((int)value.max).ToString();
        slider.curText.text = ((int)value.cur).ToString();

        int type = (int)value.CheckBoundary() + 1;
        slider.stringText.text = boundaryString[type];
        slider.slider.value = GetSliderValue(value);

        ColorBlock colorBlock = slider.slider.colors;
        colorBlock.disabledColor = sliderColor[idx];
        slider.slider.colors = colorBlock;
    }

    protected float GetSliderValue(BoundaryValue value)
    {
        float bias = ((float)value.CheckBoundary() + 1) * (1.0f / 3.0f);
        float res = bias;

        switch(value.CheckBoundary())
        {
            case BoundaryType.Less:
                res += (value.cur / value.min) / 3.0f;
                break;
            case BoundaryType.Include:
                res += (value.cur - value.min) / (value.max - value.min) / 3.0f;
                break;
            case BoundaryType.More:
                res += (value.cur - value.max) / value.min / 3.0f;
                break;
        }

        if (res > 0.99f)
            res = 0.99f;

        return res;
    }

    protected void OnDisable()
    {
        SetInitState();
    }

    protected virtual void SetInitState()
    {
        foreach (BuildingEventUIInfo uiInfo in eventUIInfos)
        {
            uiInfo.gameObject.SetActive(false);
        }

        foreach (TextMeshProUGUI text in eventTexts)
        {
            text.gameObject.SetActive(false);
        }
    }
}
