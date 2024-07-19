using UnityEngine;
using Cinemachine;
using System.Collections;


public class AdjustCinemachinePriority : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  
    public int initialPriority = 30;              
    public int targetPriority = 3;
    private void Awake()
    {
        
    }
    void Start()
    {
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.5f);

        virtualCamera.Priority = initialPriority;

        virtualCamera.Priority = targetPriority;
    }

}
