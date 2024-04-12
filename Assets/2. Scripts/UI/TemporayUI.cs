using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TemporayUI : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI text;
    public float speed;

    private void Start()
    {
        StartCoroutine(TempUpdate());
    }

    public void SetUI(string str, Vector3 pos)
    {
        text.text = str;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        RectTransform rectParent = canvas.GetComponent<RectTransform>();
        RectTransform rectSelf = GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(pos);

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectSelf.localPosition = localPos;
    }

    IEnumerator TempUpdate()
    {
        while (panel.color.a > 0)
        {
            transform.localPosition += Vector3.up * speed * 20 * Time.fixedDeltaTime;
            panel.color -= Color.black * speed * Time.fixedDeltaTime;
            text.color -= Color.black * speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }
}
