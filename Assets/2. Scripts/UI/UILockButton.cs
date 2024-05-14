using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILockButton : MonoBehaviour, IObserver
{
    [Header("Button")]
    private Button button;
    public Image buttonImage;

    [Header("Sprite")]
    public Sprite lockSprite;
    public Sprite buttonSprite;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }

    public void Notify(EventState state)
    {
        if (state == EventState.LockButton)
        {
            buttonImage.sprite = lockSprite;
            button.interactable = false;
        }
        else if(state == EventState.CityLevelUp)
        {
            buttonImage.sprite = buttonSprite;
            button.interactable = true;
        }
    }
}
