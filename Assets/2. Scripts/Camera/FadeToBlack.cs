using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
    public PostProcessVolume volume;
    private ColorGrading colorGrading;
    public float fadeDuration = 2.0f;
    public CanvasGroup uiCanvasGroup;
    public string nextSceneName;

    void Start()
    {
        volume.profile.TryGetSettings(out colorGrading);
    }

    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float currentTime = 0.0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / fadeDuration;
            colorGrading.postExposure.value = Mathf.Lerp(0, -10, normalizedTime);
            uiCanvasGroup.alpha = Mathf.Lerp(1, 0, normalizedTime);
            yield return null;
        }

        colorGrading.postExposure.value = -10;
        uiCanvasGroup.alpha = 0;


        SceneManager.LoadScene(nextSceneName);
    }
}
