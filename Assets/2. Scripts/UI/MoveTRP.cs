using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveTRP : MonoBehaviour
{
    public Transform targetObject; 
    public float moveDistance = 5f; 
    public float duration = 1f; 
    public Button myButton; 
    private bool isUp = true; 

    private void Start()
    {
        myButton.onClick.AddListener(ToggleObjectPosition);
    }

    private void ToggleObjectPosition()
    {
        if (isUp)
        {
            targetObject.DOMoveY(targetObject.position.y - moveDistance, duration).SetEase(Ease.InOutQuad);
            isUp = false; 
        }
        else
        {
            targetObject.DOMoveY(targetObject.position.y + moveDistance, duration).SetEase(Ease.InOutQuad);
            isUp = true;
        }
    }
}
