using DG.Tweening; // DOTween ���ӽ����̽� ���
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public RectTransform rectTransform;

    void Start()
    {
        // ũ�⸦ �����ϴ� �ִϸ��̼�
        rectTransform.DOSizeDelta(new Vector2(200, 200), 1.0f);
    }
}
