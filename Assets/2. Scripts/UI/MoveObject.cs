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
        else
        {
            NextButton.interactable = true; 
        }

        if (objectToMove.transform.localPosition.x >= -60)
        {
            PrevButton.interactable = false;
        }
        else
        {
            PrevButton.interactable = true;
        }

    }

    public void StartNextMoving()
    {
        StartCoroutine(MoveOverTime(-1000,1f)); 
    }

    public void StartPrevMoving()
    {

        StartCoroutine(MoveOverTime(1000, 1f)); 
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