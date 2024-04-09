using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType { Residential, Commercial, Culture, Service, Tile }

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIButtonType buttonType;
    public int index;

    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
