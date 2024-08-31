using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CreditScoreUI : MonoBehaviour, IObserver
{
    [Header("UI Component")]
    public TextMeshProUGUI MovetextObject;
    public TextMeshProUGUI ActivatetextObject;
    public Image knobImage;
    public Button quitButton;
    public Image helpImage;
    public TextMeshProUGUI helpText;

    [Header("Speed Parameter")]
    public float numberChangeSpeed = 0.1f;
    public float activateSeconds = 5.3f;
    public float fillSeconds = 2f;
    public float waitSeconds = 1f;
    public float fallSeconds = 1.3f;
    public float sizecontrollSeconds = 0.3f;

    [Header("Credit Score")]
    public Gradient scoreColor;

    IEnumerator FillKnob(float firstNum, float secondNum)
    {
        knobImage.fillAmount = firstNum / 100f;

        StartCoroutine(MoveTextDown((int)firstNum, (int)secondNum));

        knobImage.transform.localPosition = new Vector3(knobImage.transform.localPosition.x, 500.0f, knobImage.transform.localPosition.z);
        knobImage.transform.DOLocalMoveY(0.0f, fallSeconds);
        yield return new WaitForSeconds(fallSeconds + waitSeconds);

        yield return StartCoroutine(FillToAmount(secondNum / 100f));
    }

    public IEnumerator ActivateTextObject(TextMeshProUGUI ActivatetextObject, string text)
    {
        ActivatetextObject.text = text;//표시할 텍스트

        yield return new WaitForSeconds(activateSeconds);
        ActivatetextObject.gameObject.SetActive(true);
        ActivatetextObject.color -= new Color(0, 0, 0, 1);
        ActivatetextObject.DOFade(1.0f, sizecontrollSeconds);
    }

    public IEnumerator ActivateGraphicObject(Graphic targetGraphic)
    {
        yield return new WaitForSeconds(activateSeconds);
        targetGraphic.color -= new Color(0, 0, 0, 1);
        targetGraphic.gameObject.SetActive(true);
        targetGraphic.DOFade(1.0f, sizecontrollSeconds);
    }

    IEnumerator FillToAmount(float targetAmount)
    {
        float startAmount = knobImage.fillAmount;
        float elapsedTime = 0f;
        float totalChange = targetAmount - startAmount;

        AkSoundEngine.PostEvent("Play_GraphFallDown", gameObject);
        while (elapsedTime < fillSeconds)
        {
            elapsedTime += Time.deltaTime;
            float linearT = elapsedTime / fillSeconds;
            knobImage.fillAmount = startAmount + totalChange * linearT;
            knobImage.color = scoreColor.Evaluate(knobImage.fillAmount);
            MovetextObject.color = knobImage.color;
            yield return null;
        }

        knobImage.fillAmount = targetAmount;
    }

    public IEnumerator MoveTextDown(int startNumber, int endNumber)
    {
        MovetextObject.text = startNumber.ToString();
        yield return new WaitForSeconds(fallSeconds + waitSeconds);
        StartCoroutine(MoveTextOverTime(startNumber, endNumber, fillSeconds));
    }

    private IEnumerator MoveTextOverTime(int startNumber, int endNumber, float changeSeconds)
    {
        float elapsedTime = 0;
        float duration = changeSeconds;
        int currentNumber = startNumber;
        int direction = (endNumber > startNumber) ? 1 : -1;
        int totalSteps = Mathf.Abs(endNumber - startNumber);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            currentNumber = startNumber + Mathf.RoundToInt(t * totalSteps) * direction;
            MovetextObject.text = currentNumber.ToString();

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        MovetextObject.text = endNumber.ToString();
    }

    IEnumerator UnLockInput()
    {
        yield return new WaitForSeconds(activateSeconds + sizecontrollSeconds);
        InputManager.SetCanInput(true);
    }

    public void Notify(EventState state)
    {
        if (state == EventState.PayFail)
        {
            gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
            string text = "할부금을 납부하지 않아 신용 점수가 하락하였습니다.\n\n 최대한 빠르게 할부금을 납부하세요.";
            int score = RoutineManager.instance.creditRating;

            StartCoroutine(FillKnob(score + 25, score));
            StartCoroutine(ActivateTextObject(ActivatetextObject, text));
            StartCoroutine(ActivateTextObject(helpText, "닫기"));
            StartCoroutine(ActivateGraphicObject(helpImage));
            StartCoroutine(UnLockInput());
        }
        else if (state == EventState.GameOver)
        {
            gameObject.SetActive(true);
            helpImage.gameObject.SetActive(false);
            helpText.gameObject.SetActive(false);
            string text = "도시가 파산하였습니다.";
            int score = RoutineManager.instance.creditRating;

            StartCoroutine(FillKnob(score + 25, score));
            StartCoroutine(ActivateTextObject(ActivatetextObject, text));
            StartCoroutine(ActivateGraphicObject(quitButton.targetGraphic));
        }
        else
        {
            ActivatetextObject.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            helpImage.gameObject.SetActive(false);
            helpText.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
