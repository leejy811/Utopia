using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditScorePanel : MonoBehaviour
{
    public Image graphImage;
    public TextMeshProUGUI scoreText;
    public Gradient scoreColor;

    public void SetValue()
    {
        int creditRating = RoutineManager.instance.creditRating;
        float ratio = creditRating / 100.0f;
        Color color = scoreColor.Evaluate(ratio);
        graphImage.color = color;
        graphImage.fillAmount = ratio;
        scoreText.color = color;
        scoreText.text = creditRating.ToString();
    }
}
