using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CostUI : MonoBehaviour
{
    [Header("Cost")]
    public TextMeshProUGUI costText;

    public void SetValue(int cost)
    {
        costText.text = cost.ToString() + "원";

        //To Do 사회현상 전역 이벤트로 인한 건설 비용상승 추가
        /*
         * if(비용 상승중)
         * {
         *      costText.text = "<s>" + cost.ToString() + "</s> <sprite=?>" + (cost * 비용 상승량).ToString() + "원";
         * }
         * else
         * {
         *      costText.text = cost.ToString() + "원";
         * }
         */
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
