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
    public bool isFixed;

    private void Start()
    {
        StartCoroutine(TempUpdate());
    }

    public void SetUI(string str, Vector3 pos)
    {
        text.text = str;

        if (isFixed) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        RectTransform rectParent = canvas.GetComponent<RectTransform>();
        RectTransform rectSelf = GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(pos);

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        rectSelf.localPosition = localPos + new Vector2(-10, 10);
    }

    IEnumerator TempUpdate()
    {
        while (text.color.a > 0)
        {
            transform.localPosition += Vector3.up * speed * 20 * Time.fixedDeltaTime;
            text.color -= Color.black * speed * Time.fixedDeltaTime;

            if (panel != null)
                panel.color -= Color.black * speed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
