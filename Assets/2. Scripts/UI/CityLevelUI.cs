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
            cityLevelText.text += "   <sprite=0>   " + levelString[CityLevelManager.instance.levelIdx + 1];

        residentialConditionText.text = string.Format("{0:#,###}", ResidentialBuilding.cityResident) + "/" + string.Format("{0:#,###}", CityLevelManager.instance.level[CityLevelManager.instance.levelIdx].residentCondition);
        residentialConditionSlider.value = ResidentialBuilding.cityResident / (float)CityLevelManager.instance.level[CityLevelManager.instance.levelIdx].residentCondition;

        happinessConditionText.text = string.Format("{0:#,###}", RoutineManager.instance.cityHappiness) + "/" + string.Format("{0:#,###}", CityLevelManager.instance.level[CityLevelManager.instance.levelIdx].happinessCondition);
        happinessConditionSlider.value = RoutineManager.instance.cityHappiness / CityLevelManager.instance.level[CityLevelManager.instance.levelIdx].happinessCondition;

        moneyRewardText.text = string.Format("{0:#,###}", CityLevelManager.instance.level[CityLevelManager.instance.levelIdx].moneyReward);
    }
}
