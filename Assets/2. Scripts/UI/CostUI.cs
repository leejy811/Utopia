using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CostUI : MonoBehaviour
{
    [Header("Cost")]
    public TextMeshProUGUI basicCostText;
    public TextMeshProUGUI curCostText;

    public void SetValue(int cost)
    {
        basicCostText.text = cost.ToString();
        curCostText.text = cost.ToString();
    }

    public void OnUI(int cost, Vector3 pos)
    {
        SetValue(cost);

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        RectTransform rectParent = canvas.GetComponent<RectTransform>();
        RectTransform rectSelf = GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(pos);

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectSelf.localPosition = localPos;
    }
}
