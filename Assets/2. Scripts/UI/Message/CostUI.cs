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
        if (percent != 0.0f)
        {
            percent = 1.0f / percent;
        }

        costText.text = "건설 비용 : ";

        if (percent != 1.0)
            costText.text += "<s>" + (cost * percent).ToString() + "</s> <sprite=7>" + cost.ToString() + "원";
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

        transform.localPosition = localPos * (1 / 2.5f) + new Vector2(0f, -25f);
    }

    public void Notify(EventState state)
    {
        gameObject.SetActive(false);
    }
}
