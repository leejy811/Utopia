using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DayNightCycle : MonoBehaviour
{
    public PostProcessVolume volume;
    private ColorGrading colorGrading;  // 'ColorGrading' 클래스 사용
    private float transitionTime = 0.5f;
    private float timer = 0.0f;
    private bool transitioning = false;

    void Start()
    {
        if (volume.profile.TryGetSettings(out colorGrading))
        {
            colorGrading.colorFilter.value = new Color(1f, 1f, 1f);
        }
    }

    void Update()
    {
        if (transitioning)
        {
            timer += Time.deltaTime / transitionTime;
            colorGrading.colorFilter.value = Color.Lerp(new Color(1f, 1f, 1f), new Color(0.1f, 0.1f, 0.1f), timer);

            if (timer >= 1.0f)
            {
                transitioning = false;
            }
        }
    }

    public void StartTransition()
    {
        transitioning = true;
        timer = 0.0f;
    }
}
