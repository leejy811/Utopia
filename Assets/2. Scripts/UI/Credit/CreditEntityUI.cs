using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditEntityUI : MonoBehaviour
{
    public Image buildingImage;
    public TextMeshProUGUI nameText;
    public Sprite[] buildingSprites;

    public void SetValue(CreditDBEntity dbEntity)
    {
        buildingImage.sprite = buildingSprites[dbEntity.spriteIdx];
        nameText.text = dbEntity.name;
    }
}
