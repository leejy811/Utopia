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

    public IEnumerator FadeInOut(float second, bool isIn)
    {
        Color targetColor = isIn ? Color.black : Color.white;
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
}
