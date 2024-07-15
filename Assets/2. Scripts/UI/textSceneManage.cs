using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class textSceneManage : MonoBehaviour
{
    public TextMeshProUGUI MovetextObject;

    public TextMeshProUGUI BlinktextObject;

    public TextMeshProUGUI ActivatetextObject;

    public Image knobImage;

    public Button sceneButton;

    public float numberChangeSpeed = 0.1f;

    public float activateSeconds=5.3f;

    public float fillSeconds = 2f;
    public float fallSeconds = 1.3f;


    public float sizecontrollSeconds = 0.3f;

    public string sceneName;
    public float blinkDuration = 5f;
    public int blinkCount = 10;


    IEnumerator FillKnob(float firstNum, float secondNum)
    {
        knobImage.fillAmount = firstNum / 100f;

        if (knobImage != null)
        {
            RectTransform rectTransform = knobImage.GetComponent<RectTransform>();
            yield return StartCoroutine(MoveToYPosition(rectTransform, rectTransform.localPosition.y - 275, fallSeconds));

            yield return new WaitForSeconds(0f);
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }

        yield return StartCoroutine(FillToAmount(secondNum / 100f));
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
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
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


    public IEnumerator BlinkText()
    {
        if (BlinktextObject != null)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                yield return BlinktextObject.DOFade(0.1f, blinkDuration).WaitForCompletion();

                yield return BlinktextObject.DOFade(1, blinkDuration).WaitForCompletion();
            }
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”.");
        }
    }

    public IEnumerator ActivateTextObject(TextMeshProUGUI ActivatetextObject)
    {
        ActivatetextObject.text = " asdasda";
        ActivatetextObject.fontSize = 1f;


        yield return new WaitForSeconds(activateSeconds);
        if (ActivatetextObject != null)
        {
            ActivatetextObject.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(DOTween.To(() => ActivatetextObject.fontSize, x => ActivatetextObject.fontSize = x, 36f, sizecontrollSeconds));
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
    }

    public IEnumerator DeactivateTextObject(TextMeshProUGUI DeactivatetextObject)
    {
        yield return new WaitForSeconds(6f);

        if (DeactivatetextObject != null)
        {
            DeactivatetextObject.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
    }


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
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
    }

    public IEnumerator DeactivateButtonObject(Button sceneButton)
    {
        yield return new WaitForSeconds(6f);

        if (sceneButton != null)
        {
            sceneButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }


    public Graphic targetGraphic;
    public GameObject targetObject;

    public float fadeInDuration = 1.0f;
    public float visibleDuration = 2.0f;
    public float fadeOutDuration = 1.0f;

    IEnumerator FadeInOut(Graphic targetGraphic, GameObject targetObject)
    {
        targetObject.SetActive(true);

        yield return Fade(targetGraphic, 0f, 0f);

        yield return Fade(targetGraphic, 1f, fadeInDuration);

        yield return new WaitForSeconds(visibleDuration);

        yield return Fade(targetGraphic, 0f, fadeOutDuration);

        targetObject.SetActive(false);
    }

    IEnumerator Fade(Graphic targetGraphic, float targetAlpha, float duration)
    {
        float startAlpha = targetGraphic.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            targetGraphic.color = new Color(targetGraphic.color.r, targetGraphic.color.g, targetGraphic.color.b, alpha);
            yield return null;
        }
        targetGraphic.color = new Color(targetGraphic.color.r, targetGraphic.color.g, targetGraphic.color.b, targetAlpha);
    }


    void Start()
    {
        StartCoroutine(MoveTextDown(10, 20));
        StartCoroutine(BlinkText());
        StartCoroutine(FillKnob(70, 40));

        StartCoroutine(ActivateTextObject(ActivatetextObject));
        StartCoroutine(ActivateButtonObject(sceneButton));
        StartCoroutine(FadeInOut(targetGraphic, targetObject));
    }
}
