using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType { Residential, Commercial, Culture, Service, Tile }
public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIButtonType buttonType;

    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetButtonInfoPopUp((int)buttonType, rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetButtonInfoPopUp((int)buttonType, rect);
    }
}
