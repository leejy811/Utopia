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

    public float activateSeconds=5.3f;


    public float fillSeconds = 2f;
    public float fallSeconds = 3.0f;


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
            yield return rectTransform.DOLocalMoveY(rectTransform.localPosition.y - 350, fallSeconds).WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
        
        yield return StartCoroutine(FillToAmount(secondNum / 100f));
    }

    IEnumerator FillToAmount(float targetAmount)
    {
        float startAmount = knobImage.fillAmount;
        float elapsedTime = 0f;
        while (elapsedTime < fillSeconds)
        {
            elapsedTime += Time.deltaTime;
            knobImage.fillAmount = Mathf.Lerp(startAmount, targetAmount, elapsedTime / fillSeconds);
            yield return null;
        }
    }

    public IEnumerator MoveTextDown(int startNumber, int endNumber)
    {
        if (MovetextObject != null)
        {
            RectTransform rectTransform = MovetextObject.GetComponent<RectTransform>();
            MovetextObject.text = startNumber.ToString();
            yield return rectTransform.DOLocalMoveY(rectTransform.localPosition.y - 350, fallSeconds).WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
            MovetextObject.text = endNumber.ToString();
        }
        else
        {
            Debug.LogWarning("π∫∞°∞° π∫∞°¿”");
        }
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


    void Start()
    {
        StartCoroutine(MoveTextDown(10, 20));
        StartCoroutine(BlinkText());
        StartCoroutine(ActivateTextObject(ActivatetextObject));

        StartCoroutine(FillKnob(70, 40));
        StartCoroutine(ActivateButtonObject(sceneButton));
    }
}
