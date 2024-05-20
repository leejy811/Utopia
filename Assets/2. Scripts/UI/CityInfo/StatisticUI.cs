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
        float ratio = total == 0 ? 0 : value / (float)total;

        moneyText.text = value.ToString() + "원";
        ratioText.text = ((int)(ratio * 100)).ToString() + "%";
        ratioSlider.value = ratio;
    }
}

public class StatisticUI : MonoBehaviour, IObserver
{
    [Header("Top Text")]
    public TextMeshProUGUI cityLevelText;
    public TextMeshProUGUI totalTaxText;
    public TextMeshProUGUI totalSpendText;

    [Header("Tax Bar")]
    public StatisticBar residentialTaxBar;
    public StatisticBar commercialTaxBar;
    public StatisticBar cultureTaxBar;
    public StatisticBar bonusTaxBar;

    [Header("Spend Bar")]
    public StatisticBar serviceSpendBar;
    public StatisticBar tileCostBar;
    public StatisticBar residentialBonusBar;
    public StatisticBar serviceBonusBar;

    [Header("Total")]
    public TextMeshProUGUI totalCostText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI residentText;

    public bool doStamp;
    string[] levelString = { "촌락", "타운", "시티", "유토피아" };

    public void SetValue()
    {
        int totalTax = ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome;
        int totalSpend = ServiceBuilding.income + ResidentialBuilding.bonusCost + ServiceBuilding.bonusCost + Tile.income;

        cityLevelText.text = levelString[CityLevelManager.instance.levelIdx];
        totalTaxText.text = GetSignString(totalTax, "+") + "원";
        totalSpendText.text = GetSignString(totalSpend, "-") + "원";

        residentialTaxBar.SetValue(totalTax, ResidentialBuilding.income);
        commercialTaxBar.SetValue(totalTax, CommercialBuilding.income);
        cultureTaxBar.SetValue(totalTax, CultureBuilding.income);
        bonusTaxBar.SetValue(totalTax, CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome);

        serviceSpendBar.SetValue(totalSpend, ServiceBuilding.income);
        tileCostBar.SetValue(totalSpend, Tile.income);
        residentialBonusBar.SetValue(totalSpend, ResidentialBuilding.bonusCost);
        serviceBonusBar.SetValue(totalSpend, ServiceBuilding.bonusCost);

        int total = (int)((totalTax + totalSpend) * EventManager.instance.GetFinalIncomeEventValue());
        totalCostText.text = totalTax.ToString() + " " + GetSignString(totalSpend, "-") + " = " + (totalTax + totalSpend).ToString() + " * " + EventManager.instance.GetFinalIncomeEventValue().ToString() + " = " + total.ToString();
        happinessText.text = ((int)RoutineManager.instance.cityHappiness).ToString() + "%(" + Mathf.Abs((int)RoutineManager.instance.cityHappinessDifference).ToString() + "% " + (RoutineManager.instance.cityHappinessDifference > 0 ? "증가" : "감소") + ")";
        residentText.text = ResidentialBuilding.cityResident.ToString() + "명(" + Mathf.Abs(ResidentialBuilding.cityResident - ResidentialBuilding.yesterDayResident).ToString() + "명 " + ((ResidentialBuilding.cityResident - ResidentialBuilding.yesterDayResident) > 0 ? "증가" : "감소") + ")";

        Animator anim = gameObject.GetComponentInChildren<Animator>();
        anim.SetInteger("Happiness", (int)RoutineManager.instance.cityHappiness);
        anim.SetInteger("Money", total);
    }

    private string GetSignString(int data, string zeroSign)
    {
        
        if (data > 0)
            return "+ " + data.ToString();
        else if (data < 0)
            return "- " + Mathf.Abs(data).ToString();
        else
            return zeroSign + " " + data.ToString();
    }

    public void Notify(EventState state)
    {
        if(state == EventState.Statistic)
        {
            gameObject.SetActive(true);
            SetValue();
            InputManager.canInput = false;
        }
    }
}
