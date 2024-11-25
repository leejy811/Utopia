using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestPayUI : MonoBehaviour
{
    [Header("TopPanel")]
    public TextMeshProUGUI curMoneyText;
    public TextMeshProUGUI prevMoneyText;
    public TextMeshProUGUI changeText;

    [Header("BottomPanel")]
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI moneyText;
    public Slider slider;

    private void OnEnable()
    {
        SetValue();
    }

    private void SetValue()
    {
        int curMoney = ShopManager.instance.Money;

        int totalTax = ResidentialBuilding.income + CommercialBuilding.income + CultureBuilding.income + CommercialBuilding.bonusIncome + CultureBuilding.bonusIncome + ServiceBuilding.bonusIncome + ResidentialBuilding.bonusIncome;
        int totalSpend = ServiceBuilding.income + ResidentialBuilding.bonusCost + ServiceBuilding.bonusCost + CommercialBuilding.bonusCost + CultureBuilding.bonusCost + Tile.income;
        int total = totalTax + totalSpend;

        int debt = RoutineManager.instance.debt;

        DateTime payDay = RoutineManager.instance.GetPayDay();

        curMoneyText.text = GetCommaText(curMoney, 20);
        prevMoneyText.text = GetCommaText(curMoney - total);
        changeText.text = total < 0 ? "-" : "+" + GetCommaText(Mathf.Abs(total));

        balanceText.text = GetCommaText(Mathf.Max(debt - curMoney, 0)) + " 남았어요!";
        dayText.text = "만기일 : " + payDay.ToString("yy.MM.dd");
        debtText.text = GetCommaText(debt);
        moneyText.text = GetCommaText(curMoney);

        slider.value = curMoney / (float)debt;
    }

    private string GetCommaText(int data, int size = 0)
    {
        if (data == 0)
            return data.ToString() + "원";
        else if (size != 0)
            return string.Format("{0:#,###}", data) + "<size=" + size + "> 원";
        else
            return string.Format("{0:#,###}", data) + "원";
    }
}
