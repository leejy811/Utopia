using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CityLevelUI : MonoBehaviour
{
    [Header("city Level")]
    public CityType cityType;
    public TextMeshProUGUI residentialConditionText;
    public Slider residentialConditionSlider;

    public TextMeshProUGUI happinessConditionText;
    public Slider happinessConditionSlider;

    [Header("Reward")]
    public TextMeshProUGUI moneyRewardText;

    public void SetValue()
    {
        int levelIdx = (int)cityType;
        residentialConditionText.text = GetCommaText(ResidentialBuilding.cityResident) + "/" + GetCommaText(CityLevelManager.instance.level[levelIdx].residentCondition);
        residentialConditionSlider.value = ResidentialBuilding.cityResident / (float)CityLevelManager.instance.level[levelIdx].residentCondition;

        happinessConditionText.text = GetCommaText((int)RoutineManager.instance.cityHappiness) + "/" + GetCommaText(CityLevelManager.instance.level[levelIdx].happinessCondition);
        happinessConditionSlider.value = RoutineManager.instance.cityHappiness / CityLevelManager.instance.level[levelIdx].happinessCondition;

        moneyRewardText.text = " + " + GetCommaText(CityLevelManager.instance.level[levelIdx].moneyReward);
    }

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
    }
}
