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
    public int[] creditScore;

    public void SetValue()
    {
        int creditRating = RoutineManager.instance.creditRating;
        float ratio = creditScore[creditRating] / 100.0f;
        Color color = scoreColor.Evaluate(ratio);
        graphImage.color = color;
        graphImage.fillAmount = ratio;
        scoreText.color = color;
        scoreText.text = creditScore[creditRating].ToString();
    }
}
