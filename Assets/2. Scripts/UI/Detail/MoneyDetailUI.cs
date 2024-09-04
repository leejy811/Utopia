using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDetailUI : DetailUI
{
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI outcomeText;

    public override void SetValue()
    {
        int totalTax = ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome + ServiceBuilding.bonusIncome + ResidentialBuilding.bonusIncome;
        int totalSpend = ServiceBuilding.income + ResidentialBuilding.bonusCost + ServiceBuilding.bonusCost + CommercialBuilding.bonusCost + CultureBuilding.bonusCost + Tile.income;
        int total = totalTax + totalSpend;

        string sign = total >= 0 ? "+" : "-";
        string color = total >= 0 ? "<color=#00FF00>" : "<color=#FF0000>";

        totalText.text = color + sign + total.ToString() + "¿ø</color>";
        incomeText.text = totalTax.ToString() + "¿ø";
        outcomeText.text = Mathf.Abs(totalSpend).ToString() + "¿ø";
    }
}
