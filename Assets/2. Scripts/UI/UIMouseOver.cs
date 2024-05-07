using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType { Residential = 0, Commercial, Culture, Service, Tile }

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIButtonType buttonType;
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetInfoPopUp((int)buttonType, index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetInfoPopUp((int)buttonType, index);
    }
}
