using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct StatisticBar
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ratioText;
    public Slider ratioSlider;

    public void SetValue(int total, int value)
    {
        float ratio = value / total;

        moneyText.text = value.ToString() + "원";
        ratioText.text = ((int)ratio).ToString() + "%";
        ratioSlider.value = ratio;
    }
}

[System.Serializable]
public struct ComprisonUI
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI yesterdayText;
    public TextMeshProUGUI todayText;

    public void SetValue(int yesterday, int today, bool isHappiness)
    {
        resultText.text = today.ToString();
        yesterdayText.text = yesterday.ToString();
        todayText.text = yesterday.ToString() + (today - yesterday).ToString("+#;-#;0") + "=" + today.ToString();

        if (isHappiness)
        {
            resultText.text += "%";
            yesterdayText.text += "%";
            todayText.text += "%";
        }
    }
}

public class StatisticUI : MonoBehaviour
{
    [Header("Top Text")]
    public TextMeshProUGUI cityLevelText;
    public TextMeshProUGUI totalTaxText;
    public TextMeshProUGUI totalSpendText;

    [Header("Tax Bar")]
    public StatisticBar residentialTaxBar;
    public StatisticBar commercialTaxBar;
    public StatisticBar cultureTaxBar;

    [Header("Spend Bar")]
    public StatisticBar serviceSpendBar;
    public StatisticBar tileCostBar;
    public StatisticBar etcCostBar;

    [Header("Total")]
    public TextMeshProUGUI totalCostText;
    public ComprisonUI happinessComparison;
    public ComprisonUI residentComparison;

    string[] levelString = { "빌리지", "타운", "시티" };

    public void SetValue()
    {
        int totalTax = ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome;
        int totalSpend = ServiceBuilding.income + ResidentialBuilding.bonusCost + CommercialBuilding.bonusCost + CultureBuilding.bonusCost + Tile.income;

        cityLevelText.text = levelString[CityLevelManager.instance.levelIdx];
        totalTaxText.text = totalTax.ToString("+#;-#;0") + "원";
        totalSpendText.text = totalSpend.ToString("+#;-#;0") + "원";

        residentialTaxBar.SetValue(totalTax, ResidentialBuilding.income);
        commercialTaxBar.SetValue(totalTax, CommercialBuilding.income + CommercialBuilding.bonusIncome);
        cultureTaxBar.SetValue(totalTax, CultureBuilding.income + CultureBuilding.bonusIncome);

        serviceSpendBar.SetValue(totalSpend, ServiceBuilding.income);
        tileCostBar.SetValue(totalSpend, Tile.income);
        etcCostBar.SetValue(totalSpend, ResidentialBuilding.bonusCost + CommercialBuilding.bonusCost + CultureBuilding.bonusCost);

        totalCostText.text = totalTax.ToString() + totalSpend.ToString("+#;-#;0") + "=" + (totalTax - totalSpend).ToString("+#;-#;0");
        happinessComparison.SetValue((int)RoutineManager.instance.cityHappiness, (int)(RoutineManager.instance.cityHappiness - RoutineManager.instance.cityHappinessDifference), true);
        residentComparison.SetValue(ResidentialBuilding.cityResident, ResidentialBuilding.cityResident - ResidentialBuilding.residentReduction, false);
    }
}
