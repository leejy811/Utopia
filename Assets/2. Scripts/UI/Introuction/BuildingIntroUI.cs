using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
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

    [Header("Slider Info")]
    public string[] boundaryString;
    [Range(0.0f, 1.0f)] public float[] sliderValue;
    public Color[] sliderColor;

    string[] typeString = { "�ְ�", "���", "��ȭ", "����" };
    string[] subTypeString = { "����Ʈ", "��ȭ", "��ȭ", "����", "����", "�̼�", "�ҹ�", "����" };

    public virtual void SetValue(Building building)
    {
        buildingNameText.text = building.buildingName + building.count;
        buildingInfoText.text = typeString[(int)building.type] + "/" + subTypeString[(int)building.subType] + "/" + building.grade + "���";
        happinessText.text = "<sprite=" + Mathf.Min(building.happinessRate / 20, 4) + "> " + building.happinessRate + "(" + GetSignString(building.happinessDifference, "+") + ")%";

        foreach(var uiInfo in eventUIInfos)
        {
            uiInfo.gameObject.SetActive(false);
        }

        for (int i = 0;i < building.curEvents.Count; i++)
        {
            int idx = building.curEvents[i].type == EventType.Event ? 2 : 0;
            eventUIInfos[idx + i].gameObject.SetActive(true);
            eventUIInfos[idx + i].SetEventUIInfo(building.curEvents[i]);
        }
    }

    public void OnUI(Building building, Vector3 pos)
    {
        transform.localPosition = pos;
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

    protected void SetSlider(ValueSlider slider, BoundaryValue value)
    {
        slider.minText.text = value.min.ToString();
        slider.maxText.text = value.max.ToString();
        slider.curText.text = value.cur.ToString();

        int type = (int)value.CheckBoundary() + 1;
        slider.stringText.text = boundaryString[type];
        slider.slider.value = sliderValue[type];

        ColorBlock colorBlock = slider.slider.colors;
        colorBlock.disabledColor = sliderColor[type];
        slider.slider.colors = colorBlock;
    }

    private void OnDisable()
    {
        foreach(BuildingEventUIInfo uiInfo in eventUIInfos)
        {
            uiInfo.gameObject.SetActive(false);
        }
    }
}
