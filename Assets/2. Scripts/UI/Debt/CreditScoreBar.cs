using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditScoreBar : MonoBehaviour
{
    [Header("UI Element")]
    public Image scoreImage;
    public TextMeshProUGUI scoreText;

    [Header("value")]
    public Gradient scoreColor;
    public float fillSecond;

    public void SetScore(int startScore, int endScore)
    {
        StartCoroutine(FillKnob(startScore, endScore));
        StartCoroutine(MoveTextOverTime(startScore, endScore));
    }

    IEnumerator FillKnob(int firstNum, int secondNum)
    {
        scoreImage.fillAmount = firstNum / 100.0f;

        float startAmount = scoreImage.fillAmount;
        float endAmount = secondNum / 100.0f;
        float elapsedTime = 0f;
        float totalChange = endAmount - startAmount;

        while (elapsedTime < fillSecond)
        {
            elapsedTime += Time.deltaTime;
            float linearT = elapsedTime / fillSecond;
            scoreImage.fillAmount = startAmount + totalChange * linearT;
            scoreImage.color = scoreColor.Evaluate(scoreImage.fillAmount);
            scoreText.color = scoreImage.color;
            yield return null;
        }

        scoreImage.fillAmount = endAmount;
    }

    private IEnumerator MoveTextOverTime(int startNumber, int endNumber)
    {
        float elapsedTime = 0;
        int currentNumber = startNumber;
        int direction = (endNumber > startNumber) ? 1 : -1;
        int totalSteps = Mathf.Abs(endNumber - startNumber);

        while (elapsedTime < fillSecond)
        {
            float t = elapsedTime / fillSecond;
            currentNumber = startNumber + Mathf.RoundToInt(t * totalSteps) * direction;
            scoreText.text = currentNumber.ToString();

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        scoreText.text = endNumber.ToString();
    }
}
