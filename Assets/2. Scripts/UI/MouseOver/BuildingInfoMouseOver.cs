using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType { Residential = 0, Commercial, Culture, Service, Tile }

public class BuildingInfoMouseOver : UIMouseOver
{
    public UIButtonType buttonType;
    public int index;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        UIManager.instance.SetInfoPopUp((int)buttonType, index);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.SetInfoPopUp((int)buttonType, index);
    }
}
