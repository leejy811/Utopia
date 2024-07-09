using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class FadeInFromBlack : MonoBehaviour
{
    public PostProcessVolume volume;
    private ColorGrading colorGrading;
    public float fadeDuration = 1.0f; 

    void Start()
    {
        volume.profile.TryGetSettings(out colorGrading);
        colorGrading.postExposure.value = -10; 
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float currentTime = 0.0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / fadeDuration;
            colorGrading.postExposure.value = Mathf.Lerp(-10, 0, normalizedTime);
            yield return null;
        }

        colorGrading.postExposure.value = 0;
    }
}
