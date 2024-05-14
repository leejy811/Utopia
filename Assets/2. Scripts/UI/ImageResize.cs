using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class ImageResize : MonoBehaviour
{
    public Image imageToResize; 
    public Vector2 targetSize; 
    public float duration = 2f; 

    void Start()
    {

        imageToResize.rectTransform.DOSizeDelta(targetSize, duration).SetEase(Ease.InOutQuad);
    }
}
