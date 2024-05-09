using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CityLevelUI : MonoBehaviour
{
    [Header("city Level")]
    public TextMeshProUGUI cityLevelText;

    public TextMeshProUGUI residentialConditionText;
    public Slider residentialConditionSlider;

    public TextMeshProUGUI happinessConditionText;
    public Slider happinessConditionSlider;

    [Header("Reward Building")]


    [Header("Reward")]
    public TextMeshProUGUI moneyRewardText;

    string[] levelString = { "빌리지", "타운", "시티" };

    public void SetValue()
    {
        cityLevelText.text = levelString[CityLevelManager.instance.levelIdx];

        if (CityLevelManager.instance.levelIdx < levelString.Length - 1)
            cityLevelText.text += "   <sprite=8>   " + levelString[CityLevelManager.instance.levelIdx + 1];

        residentialConditionText.text = GetCommaText(ResidentialBuilding.cityResident) + "/" + GetCommaText(CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].residentCondition);
        residentialConditionSlider.value = ResidentialBuilding.cityResident / (float)CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].residentCondition;

        happinessConditionText.text = GetCommaText((int)RoutineManager.instance.cityHappiness) + "/" + GetCommaText(CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].happinessCondition);
        happinessConditionSlider.value = RoutineManager.instance.cityHappiness / CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].happinessCondition;

        moneyRewardText.text = " + " + GetCommaText(CityLevelManager.instance.level[CityLevelManager.instance.levelIdx + 1].moneyReward);
    }

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
    }
}
