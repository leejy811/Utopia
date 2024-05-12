using DG.Tweening; // DOTween 네임스페이스 사용
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public RectTransform rectTransform;

    void Start()
    {
        // 크기를 변경하는 애니메이션
        rectTransform.DOSizeDelta(new Vector2(200, 200), 1.0f);
    }
}
