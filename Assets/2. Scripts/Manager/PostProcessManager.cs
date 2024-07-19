using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager instance;

    public PostProcessVolume postProcess;

    private ColorGrading colorGrading;
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
    }

    private void Start()
    {
        postProcess.profile.TryGetSettings(out colorGrading);
        postProcess.profile.TryGetSettings(out vignette);
    }

    private void Update()
    {
        if (isFade) return;

        float angleX = RoutineManager.instance.mainLight.transform.localEulerAngles.x;

        if (angleX > 0.0f && angleX < 45.0f)
            colorGrading.colorFilter.value = Color.white * (((angleX / 45.0f) * 0.7f) + 0.3f);
        else if (angleX >= 45.0f && angleX < 90.0f)
            colorGrading.colorFilter.value = Color.white;
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
            vignette.intensity.Interp(vignette.intensity, targetValue, t);
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
