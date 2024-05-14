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
        StartCoroutine(DelayedAction());

    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(1f);

        virtualCamera.Priority = initialPriority;

        virtualCamera.Priority = targetPriority;
    }

}
