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
    public TextMeshProUGUI curFeeText;

    [Header("Hover Chart Info")]
    public TextMeshProUGUI chartDayText;
    public TextMeshProUGUI chartCostText;
    public GameObject infoPanel;
    public GameObject emptyPanel;

    [Header("Trade Info")]
    public TextMeshProUGUI tradeChipText;
    public TextMeshProUGUI tradeCostText;
    public TextMeshProUGUI tradeFeeText;

    [Header("ErrorMsg")]
    public Image errorPanel;
    public TextMeshProUGUI errorText;
    public float errorFadeIn;
    public float errorFadeOut;

    [Header("TradeMsg")]
    public Image tradePanel;
    public TextMeshProUGUI tradeText;
    public float tradeFadeIn;
    public float tradeFadeOut;

    private bool isTweening;
    private int tradeChip;

    private void OnEnable()
    {
        tradeChip = 0;

        SetCurInfo();
        SetChartInfo(DateTime.MinValue, false);
        SetTradeInfo();
    }

    private void SetCurInfo()
    {
        curMoneyText.text = ShopManager.instance.Money.ToString("#,##0") + " 원";
        curChipText.text = ChipManager.instance.CurChip.ToString("#,##0") + " 개";
        curChipCostText.text = ChipManager.instance.CalcChipCost().ToString("#,##0") + " 원";
        curChipRatioText.text = GetRatioText(RoutineManager.instance.day);

        string feeText = "칩 거래 수수료 : " + ChipManager.instance.GetFee().ToString() + "%";

        int additionalFee = 0;
        foreach (Event e in EventManager.instance.globalEvents)
        {
            if (e.valueType == ValueType.Chip)
                additionalFee += e.effectValue[0];
        }

        if (additionalFee < 0)
            feeText += "<color=#5480D1>(" + additionalFee + "%)";
        else if (additionalFee > 0)
            feeText += "<color=#FE5C56>(+" + additionalFee + "%)";
        curFeeText.text = feeText;
    }

    public void SetChartInfo(DateTime hoverDay, bool isHover)
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
        tradeCostText.text = (ChipManager.instance.CalcChipCost() * tradeChip).ToString("#,##0") + " 원";
        tradeFeeText.text = "+" + (ChipManager.instance.CalcChipCostWithFee(tradeChip) - (ChipManager.instance.CalcChipCost() * tradeChip)).ToString("#,##0") + " 원";
    }

    private string GetRatioText(DateTime day)
    {
        float curCost = ChipManager.instance.chipCostDatas[day];
        float prevCost = ChipManager.instance.chipCostDatas[day.Subtract(new TimeSpan(1, 0, 0, 0))];

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
        tradeChip = 0;
        SetTradeInfo();
    }

    public void OnClickTrade(bool isBuy)
    {
        int tradeAmount = tradeChip * (isBuy ? 1 : -1);

        if (tradeAmount == 0)
            return;
        else if (ChipManager.instance.CurChip + tradeAmount < 0)
        {
            OnMessage(errorPanel, errorText, "칩이 부족합니다", errorFadeIn, errorFadeOut);
            return;
        }
        else if (ShopManager.instance.Money - ChipManager.instance.CalcChipCostWithFee(tradeAmount) < 0)
        {
            OnMessage(errorPanel, errorText, "돈이 부족합니다", errorFadeIn, errorFadeOut);
            return;
        }

        string resMsg = "칩 " + Mathf.Abs(tradeAmount).ToString() + "개를 " + Mathf.Abs(ChipManager.instance.CalcChipCostWithFee(tradeAmount)).ToString()
                        + "원에 " + (tradeAmount > 0 ? "구매" : "판매") + "했습니다.";
        OnMessage(tradePanel, tradeText, resMsg, tradeFadeIn, tradeFadeOut);
        ChipManager.instance.TradeChip(tradeAmount);
        tradeChip = 0;
        SetCurInfo();
        SetTradeInfo();
    }

    private void OnMessage(Image panel, TextMeshProUGUI text, string msg, float fadeInTime, float fadeOutTime)
    {
        if (isTweening) return;

        isTweening = true;
        text.text = msg;

        text.DOFade(1.0f, fadeInTime);
        panel.DOFade(1.0f, fadeInTime);
        panel.transform.DOLocalMoveX(panel.transform.localPosition.x + 15.0f, fadeInTime).OnComplete(() =>
        {
            text.DOFade(0.0f, fadeOutTime);
            panel.DOFade(0.0f, fadeOutTime).OnComplete(() =>
            {
                panel.transform.localPosition -= Vector3.right * 15.0f;
                isTweening = false;
            });
        });
    }
}
