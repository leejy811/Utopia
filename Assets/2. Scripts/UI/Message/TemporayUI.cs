using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum TextType { EventName, EventDescription }

public class TemporayUI : MonoBehaviour
{
    public Image[] images;
    public TextMeshProUGUI[] texts;
    public float fadeInTime;
    public float waitTime;
    public float fadeOutTime;
    public bool isFixed;

    private void Start()
    {
        StartCoroutine(TempUpdate());
    }

    public void SetUI(string[] str, Vector3 pos)
    {
        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].text = str[i];
        }

        if (isFixed) return;

        SetPosition(pos);
    }

    public void SetUI(Event curEvent, Vector3 pos)
    {
        texts[(int)TextType.EventName].text = curEvent.eventName;
        texts[(int)TextType.EventDescription].text = curEvent.eventEffectComment;
        images[0].sprite = curEvent.eventIcon;

        if (isFixed) return;

        SetPosition(pos);
    }

    private void SetPosition(Vector3 pos)
    {
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
        SetAlpha(0.0f);

        float color = 0.0f;
        float speed = fadeInTime != 0 ? 1.0f / fadeInTime : 0.0f;

        yield return StartCoroutine(UpdateColor(color, speed));

        yield return new WaitForSeconds(waitTime);

        SetAlpha(1.0f);

        color = 1.0f;
        speed = fadeOutTime != 0 ? 1.0f / fadeOutTime : 0.0f;

        yield return StartCoroutine(UpdateColor(color, speed));

        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        foreach (TextMeshProUGUI text in texts)
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        foreach (Image image in images)
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    IEnumerator UpdateColor(float color, float speed)
    {
        int sign = color == 0.0f ? 1 : -1;

        while (true)
        {
            if (speed == 0.0f) break;
            else if (sign == 1 && color >= 1.0f) break;
            else if (sign == -1 && color <= 0.0f) break;

            color += speed * sign * Time.fixedDeltaTime;
            transform.localPosition += Vector3.up * speed * 20 * Time.fixedDeltaTime;
            SetAlpha(color);

            yield return new WaitForFixedUpdate();
        }
    }
}
