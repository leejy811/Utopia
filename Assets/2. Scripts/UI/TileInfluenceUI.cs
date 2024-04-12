using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.PlayerSettings;

public class TileInfluenceUI : MonoBehaviour
{
    [Header("Influence")]
    public TextMeshProUGUI[] influenceText;

    string[] influenceString = { "주거", "상업", "문화", "서비스" };

    public void SetValue(Tile tile)
    {
        for(int i = 0;i < influenceText.Length;i++)
        {
            influenceText[i].text = influenceString[i] + ": " + tile.influenceValues[i].ToString();
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
