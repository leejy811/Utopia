using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDetailUI : DetailUI
{
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI outcomeText;

    public override void SetValue()
    {
        int totalTax = ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome;
        int totalSpend = ServiceBuilding.income + ResidentialBuilding.bonusCost + ServiceBuilding.bonusCost + Tile.income;

        incomeText.text = totalTax.ToString() + "¿ø";
        outcomeText.text = Mathf.Abs(totalSpend).ToString() + "¿ø";
    }
}
