using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CostUI : MonoBehaviour, IObserver
{
    [Header("Cost")]
    public TextMeshProUGUI costText;

    public void SetValue(int cost)
    {
        float percent = EventManager.instance.GetBuildCostEventValue();

        costText.text = "건설 비용 : ";

        if (percent != 1.0)
            costText.text += "<s>" + cost.ToString() + "</s> <sprite=7>" + (cost * percent).ToString() + "원";
        else
            costText.text += cost.ToString() + "원";
    }

    public void OnUI(int cost, Vector3 pos)
    {
        gameObject.SetActive(true);

        SetValue(cost);

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        RectTransform rectParent = canvas.GetComponent<RectTransform>();
        RectTransform rectSelf = GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(pos);

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectSelf.localPosition = localPos + new Vector2(0, -7);
    }

    public void Notify(EventState state)
    {
        gameObject.SetActive(false);
    }
}
