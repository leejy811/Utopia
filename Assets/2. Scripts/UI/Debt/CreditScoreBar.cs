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
    public float waitSecond;
    public float fillSecond;

    public void SetScore(int startScore, int endScore)
    {
        if (startScore > endScore)
        {
            AkSoundEngine.PostEvent("Play_GraphFallDown", gameObject);
        }
        else if (startScore < endScore)
        {
            AkSoundEngine.PostEvent("Play_GraphUP", gameObject);
        }
        StartCoroutine(FillKnob(startScore, endScore));
        StartCoroutine(MoveTextOverTime(startScore, endScore));
    }

    IEnumerator FillKnob(int firstNum, int secondNum)
    {
        SetScoreImage(firstNum / 100.0f);

        float startAmount = scoreImage.transform.localScale.x;
        float endAmount = secondNum / 100.0f;
        float elapsedTime = 0f;
        float totalChange = endAmount - startAmount;

        yield return new WaitForSeconds(waitSecond);

        while (elapsedTime < fillSecond)
        {
            elapsedTime += Time.deltaTime;
            float linearT = elapsedTime / fillSecond;
            SetScoreImage(startAmount + totalChange * linearT);
            yield return null;
        }

        scoreImage.transform.localScale = Vector3.one * endAmount;
    }

    private void SetScoreImage(float amount)
    {
        scoreImage.transform.localScale = Vector3.one * amount;
        scoreImage.color = scoreColor.Evaluate(amount);
    }

    private IEnumerator MoveTextOverTime(int startNumber, int endNumber)
    {
        float elapsedTime = 0;
        int currentNumber = startNumber;
        int direction = (endNumber > startNumber) ? 1 : -1;
        int totalSteps = Mathf.Abs(endNumber - startNumber);
        scoreText.text = currentNumber.ToString();

        yield return new WaitForSeconds(waitSecond);

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
