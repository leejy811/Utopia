using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListUI : MonoBehaviour
{
    [Header("Cost")]
    public TextMeshProUGUI[] costText;

    [Header("Button")]
    public Button[] Button;
    public Image[] ButtonImage;
    public Sprite lockSprite;

    public virtual void SetValue(int type)
    {

    }
}
