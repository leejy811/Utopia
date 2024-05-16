using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string soundName;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (soundName.Length > 0)
            AkSoundEngine.PostEvent(soundName, gameObject);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
