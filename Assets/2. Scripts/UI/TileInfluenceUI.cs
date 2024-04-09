using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileInfluenceUI : MonoBehaviour
{
    [Header("Influence")]
    public TextMeshProUGUI[] influenceText;

    public void SetValue(Tile tile)
    {
        for(int i = 0;i < influenceText.Length;i++)
        {
            influenceText[i].text = tile.influenceValues[i].ToString();
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        RectTransform rectParent = canvas.GetComponent<RectTransform>();
        RectTransform rectSelf = GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(tile.transform.position);

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectSelf.localPosition = localPos;
    }
}
