using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChipLobbyUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI tradeCountText;
    public TextMeshProUGUI errorMsgText;

    [Header("Panel")]
    public GameObject buyPanel;
    public GameObject sellPanel;

    private void OnEnable()
    {
        tradeCountText.text = ChipManager.instance.curTradeTimes.ToString() + " / " + ChipManager.instance.maxTradeTimes.ToString();
        errorMsgText.color -= new Color(0, 0, 0, 1);
    }

    public void OnClickTradeButton(bool isBuy)
    {
        if (ChipManager.instance.curTradeTimes == 0)
        {
            errorMsgText.color += new Color(0, 0, 0, 1);
            errorMsgText.DOFade(0.0f, 0.5f);
            return;
        }

        buyPanel.SetActive(isBuy);
        sellPanel.SetActive(!isBuy);
    }
}
