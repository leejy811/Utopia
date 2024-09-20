using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager instance;

    public Volume volume;
    public CanvasGroup canvas;

    private ColorAdjustments colorGrading;
    private Vignette vignette;

    private float brightness;
    private bool isFade;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetComponent();
    }

    private void Update()
    {
        if (isFade) return;

        if (SceneManager.GetActiveScene().name == "LobbyScene") return;

        float angleX = RoutineManager.instance.mainLight.transform.localEulerAngles.x;

        if (angleX > 0.0f && angleX < 45.0f)
            colorGrading.colorFilter.value = Color.white * (((angleX / 45.0f) * 0.7f) + 0.3f);
        else if (angleX >= 45.0f && angleX < 90.0f)
            colorGrading.colorFilter.value = Color.white;
    }

    public void SetComponent()
    {
        volume = FindObjectOfType<Volume>();
        canvas = FindObjectOfType<CanvasGroup>();

        volume.profile.TryGet(out colorGrading);
        volume.profile.TryGet(out vignette);
    }

    public IEnumerator FadeInOut(float second, bool isIn)
    {
        Color targetColor = isIn ? Color.black : Color.white * brightness;
        float curTime = 0;
        float t = 0;
        brightness = colorGrading.colorFilter.value.grayscale;
        if(isIn) isFade = true;

        while (t < 1f)
        {
            curTime += Time.fixedDeltaTime;
            t = curTime / second;
            colorGrading.colorFilter.Interp(colorGrading.colorFilter.value, targetColor, t);
            canvas.alpha = Mathf.Lerp(canvas.alpha, Convert.ToInt32(!isIn), t);
            yield return new WaitForFixedUpdate();
        }

        if (!isIn) isFade = false;
    }

    public IEnumerator VignetteInOut(float second, float targetValue)
    {
        float curTime = 0;
        float t = 0;
        while (t < 1f)
        {
            curTime += Time.fixedDeltaTime;
            t = curTime / second;
            vignette.intensity.Interp(vignette.intensity.value, targetValue, t);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator FadeUpdate(float second, float value)
    {
        Color targetColor = Color.white * value;
        float curTime = 0;
        float t = 0;

        while (t < 1f)
        {
            curTime += Time.fixedDeltaTime;
            t = curTime / second;
            colorGrading.colorFilter.Interp(colorGrading.colorFilter.value, targetColor, t);
            yield return new WaitForFixedUpdate();
        }
    }
}
