using UnityEngine;
using Cinemachine;
using System.Collections;


public class AdjustCinemachinePriority : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  
    public int initialPriority = 30;              
    public int targetPriority = 3;

    void Start()
    {
        StartCoroutine(DelaySetPriority());
    }

    IEnumerator DelaySetPriority()
    {
        yield return new WaitForSeconds(0.2f);

        virtualCamera.Priority = initialPriority;

        virtualCamera.Priority = targetPriority;

        AkSoundEngine.PostEvent("Play_Intro_camera", gameObject);
    }
}
