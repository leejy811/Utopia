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
    [Range(0.0f, 1.0f)] public float[] sliderValue;
    public Color[] sliderColor;

    protected string[] typeString = { "주거", "상업", "문화", "서비스" };
    string[] subTypeString = { "아파트", "잡화", "영화", "경찰", "음식", "미술", "소방", "여가", "의료", "운동" };
    
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
            eventTexts[i].text = "사회현상 발생으로 인해 " + building.curEvents[i].GetEventToString();
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

    protected void SetSlider(ValueSlider slider, BoundaryValue value)
    {
        slider.minText.text = ((int)value.min).ToString();
        slider.maxText.text = ((int)value.max).ToString();
        slider.curText.text = ((int)value.cur).ToString();

        int type = (int)value.CheckBoundary() + 1;
        slider.stringText.text = boundaryString[type];
        slider.slider.value = sliderValue[type];

        ColorBlock colorBlock = slider.slider.colors;
        colorBlock.disabledColor = sliderColor[type];
        slider.slider.colors = colorBlock;
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
