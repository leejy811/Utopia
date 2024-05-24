using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 기능 사용을 위해 추가

public class MoveObject : MonoBehaviour
{
    public GameObject objectToMove; 

    public Button NextButton;
    public Button PrevButton;

    void Update()
    {
        CheckPositionAndDisableButton();
    }

    void CheckPositionAndDisableButton()
    {
        if (objectToMove.transform.localPosition.x <= -2900)
        {
            NextButton.interactable = false;
        }

        if (objectToMove.transform.localPosition.x >= -1040)
        {
            PrevButton.interactable = false;
        }

    }

    public void StartNextMoving()
    {
        StartCoroutine(DisableButtonsTemporarily());
        StartCoroutine(MoveOverTime(-1000, 1f));
    }

    public void StartPrevMoving()
    {
        StartCoroutine(DisableButtonsTemporarily());
        StartCoroutine(MoveOverTime(1000, 1f));
    }


    IEnumerator DisableButtonsTemporarily()
    {
        NextButton.interactable = false;
        PrevButton.interactable = false;
        yield return new WaitForSeconds(1f);
        NextButton.interactable = true;
        PrevButton.interactable = true;
    }


    IEnumerator MoveOverTime(float targetX, float duration)
    {
        Vector3 startPosition = objectToMove.transform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x + targetX, startPosition.y, startPosition.z);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newX = Mathf.Lerp(startPosition.x, endPosition.x, Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / duration));
            objectToMove.transform.localPosition = new Vector3(newX, startPosition.y, startPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.transform.localPosition = endPosition;


    }
}