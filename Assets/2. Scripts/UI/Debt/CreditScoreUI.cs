using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScoreUI : MonoBehaviour, IObserver
{
    [Header("UI Component")]
    public TextMeshProUGUI MovetextObject;
    public TextMeshProUGUI ActivatetextObject;
    public Image knobImage;
    public Button sceneButton;

    [Header("Speed Parameter")]
    public float numberChangeSpeed = 0.1f;
    public float activateSeconds = 5.3f;
    public float fillSeconds = 2f;
    public float fallSeconds = 1.3f;
    public float sizecontrollSeconds = 0.3f;

    [Header("Credit Score")]
    public int[] creditScore;
    public Gradient scoreColor;

    // �ּ� ��ǥ�� �Լ��� ���� �Լ�, ��� X


    /// <summary>
    ///  �ſ����� �׷��� �� ����ǥ��
    /// </summary>
    /// <param name="firstNum"></���� ����(�ſ�����)>
    /// <param name="secondNum"></������ ����(�ſ�����)>
    /// <returns></returns>
    IEnumerator FillKnob(float firstNum, float secondNum)
    {
        knobImage.fillAmount = firstNum / 100f;

        if (knobImage != null)
        {
            RectTransform rectTransform = knobImage.GetComponent<RectTransform>();
            StartCoroutine(MoveTextDown((int)firstNum, (int)secondNum));
            yield return StartCoroutine(MoveToYPosition(rectTransform, rectTransform.localPosition.y - 275, fallSeconds));

            yield return new WaitForSeconds(0f);
        }
        else
        {
            Debug.LogWarning("������ ������");
        }

        yield return StartCoroutine(FillToAmount(secondNum / 100f));
    }

    /// <summary>
    ///  �ſ����� ���� �� �ؽ�Ʈ ���� �Լ�
    /// </summary>
    public IEnumerator ActivateTextObject(TextMeshProUGUI ActivatetextObject, string text)
    {
        ActivatetextObject.text = text;//ǥ���� �ؽ�Ʈ
        ActivatetextObject.fontSize = 1f;

        yield return new WaitForSeconds(activateSeconds);
        if (ActivatetextObject != null)
        {
            ActivatetextObject.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(DOTween.To(() => ActivatetextObject.fontSize, x => ActivatetextObject.fontSize = x, 15f, sizecontrollSeconds));
        }
        else
        {
            Debug.LogWarning("������ ������");
        }
    }

    /// <summary>
    ///  �ſ����� ���� �� ��ư ���� �Լ�
    /// </summary>
    public IEnumerator ActivateButtonObject(Button sceneButton)
    {
        yield return new WaitForSeconds(activateSeconds);
        if (sceneButton != null)
        {
            sceneButton.transform.localScale = Vector3.zero;
            sceneButton.gameObject.SetActive(true);
            sceneButton.transform.DOScale(Vector3.one, sizecontrollSeconds);
        }
        else
        {
            Debug.LogWarning("������ ������");
        }
    }

    IEnumerator MoveToYPosition(RectTransform rectTransform, float targetY, float duration)
    {
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, targetY, startPosition.z);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float linearT = elapsedTime / duration;
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, linearT);
            yield return null;
        }

        rectTransform.localPosition = endPosition;
    }

    IEnumerator FillToAmount(float targetAmount)
    {
        float startAmount = knobImage.fillAmount;
        float elapsedTime = 0f;
        float totalChange = targetAmount - startAmount;

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
        if (MovetextObject != null)
        {
            RectTransform rectTransform = MovetextObject.GetComponent<RectTransform>();
            MovetextObject.text = startNumber.ToString();
            yield return StartCoroutine(MoveToYPosition(rectTransform, rectTransform.localPosition.y - 275, fallSeconds));
            yield return new WaitForSeconds(0f);
            yield return StartCoroutine(MoveTextOverTime(startNumber, endNumber, fillSeconds));
        }
        else
        {
            Debug.LogWarning("������ ������");
        }
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
        InputManager.canInput = true;
    }

    public void Notify(EventState state)
    {
        if (state == EventState.CreditScore)
        {
            gameObject.SetActive(true);
            sceneButton.gameObject.SetActive(false);
            string text = "�Һα��� �������� �ʾ� �ſ� ������ �϶��Ͽ����ϴ�.\n\n �ִ��� ������ ����� �����ϼ���.";
            int creditRating = RoutineManager.instance.creditRating;

            StartCoroutine(FillKnob(creditScore[creditRating - 1], creditScore[creditRating]));
            StartCoroutine(ActivateTextObject(ActivatetextObject, text));
            StartCoroutine(UnLockInput());
        }
        else if (state == EventState.GameOver)
        {
            gameObject.SetActive(true);
            string text = "���ð� �Ļ��Ͽ����ϴ�.";
            int creditRating = RoutineManager.instance.creditRating;

            StartCoroutine(FillKnob(creditScore[creditRating - 1], creditScore[creditRating]));
            StartCoroutine(ActivateTextObject(ActivatetextObject, text));
            StartCoroutine(ActivateButtonObject(sceneButton));
        }
        else
        {
            knobImage.transform.localPosition = new Vector3(0, 366, 0);
            MovetextObject.transform.localPosition = new Vector3(0, 366, 0);
            ActivatetextObject.gameObject.SetActive(false);
            sceneButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
