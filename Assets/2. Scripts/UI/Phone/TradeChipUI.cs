using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeChipUI : MonoBehaviour
{
    [Header("Cur Info")]
    public TextMeshProUGUI curMoneyText;
    public TextMeshProUGUI curChipText;
    public TextMeshProUGUI curChipCostText;
    public TextMeshProUGUI curChipRatioText;

    [Header("Hover Chart Info")]
    public TextMeshProUGUI chartDayText;
    public TextMeshProUGUI chartCostText;
    public GameObject infoPanel;
    public GameObject emptyPanel;

    [Header("Trade Info")]
    public TextMeshProUGUI tradeChipText;
    public TextMeshProUGUI tradeCostText;

    [Header("ErrorMsg")]
    public Image errorPanel;
    public TextMeshProUGUI errorText;
    public float errorSecond;

    private int tradeChip;
    private DateTime hoverDay;
    private bool isHover;

    private void OnEnable()
    {
        tradeChip = 0;
        isHover = false;

        SetCurInfo();
        SetChartInfo();
        SetTradeInfo();
    }

    private void SetCurInfo()
    {
        curMoneyText.text = ShopManager.instance.Money.ToString("#,##0") + " 원";
        curChipText.text = ChipManager.instance.curChip.ToString("#,##0") + " 개";
        curChipCostText.text = ChipManager.instance.CalcChipCost().ToString("#,##0") + " 원";
        curChipRatioText.text = GetRatioText(RoutineManager.instance.day);
    }

    private void SetChartInfo()
    {
        infoPanel.SetActive(isHover);
        emptyPanel.SetActive(!isHover);

        if (!isHover) return;
        chartDayText.text = hoverDay.ToString("yy.MM.dd");
        chartCostText.text = ChipManager.instance.chipCostDatas[hoverDay].ToString("#,##0") + " 원 (" + GetRatioText(hoverDay) + ")";
    }

    private void SetTradeInfo()
    {
        tradeChipText.text = tradeChip.ToString("#,##0") + "개";
        tradeCostText.text = (tradeChip * ChipManager.instance.CalcChipCost()).ToString("#,##0") + " 원";
    }

    private string GetRatioText(DateTime day)
    {
        float curCost = ChipManager.instance.chipCostDatas[RoutineManager.instance.day];
        float prevCost = ChipManager.instance.chipCostDatas[RoutineManager.instance.day.Subtract(new TimeSpan(1, 0, 0, 0))];

        string sign = (curCost / prevCost) > 1.0f ? "+ " : "- ";
        string text = sign + ((int)(Mathf.Abs((curCost / prevCost) - 1.0f) * 100.0f)).ToString() + "%";

        return text;
    }

    public void OnClickAddChip(int amount)
    {
        tradeChip += amount;
        SetTradeInfo();
    }

    public void OnClickResetChip()
    {
        tradeChip += 0;
        SetTradeInfo();
    }

    public void OnClickTrade(bool isBuy)
    {
        tradeChip *= isBuy ? 1 : -1;

        if (ChipManager.instance.curChip + tradeChip > 0)
        {
            OnErrorMsg("칩이 부족합니다", errorSecond);
            return;
        }
        else if (ShopManager.instance.Money - (ChipManager.instance.CalcChipCost() * tradeChip) < 0)
        {
            OnErrorMsg("돈이 부족합니다", errorSecond);
            return;
        }

        ChipManager.instance.TradeChip(tradeChip);
        tradeChip = 0;
        SetCurInfo();
        SetTradeInfo();
    }

    private void OnErrorMsg(string msg, float second)
    {
        errorText.text = msg;

        errorText.color += Color.black;
        errorPanel.color += Color.black;

        errorText.DOFade(0.0f, second);
        errorPanel.DOFade(0.0f, second);
    }
}
