using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweeningObject : MonoBehaviour
{
    public RectTransform firstImageRectTransform;
    public RectTransform secondImageRectTransform;


    public void MatchSizes()
    {
        Vector2 targetSize = firstImageRectTransform.sizeDelta;
        Debug.Log($"sdasdas: {targetSize}");

        Vector2 targetSize2 = secondImageRectTransform.sizeDelta;
        Debug.Log($"Final size: {targetSize2}");
        Debug.Log("Starting size change animation");
        firstImageRectTransform.DOSizeDelta(new Vector2(550, 50), 2f).OnComplete(() => {
            Debug.Log("Animation completed");
        });

    }
}