using UnityEngine;
using Cinemachine;

public class CameraPriorityController : MonoBehaviour
{
    public CinemachineCameraController cameraController;
    public Transform sourceCameraTransform;
    public CinemachineVirtualCamera virtualCamera2;

    private int defaultPriority = 5;
    private int activePriority = 20;
    private bool isActive = false;
    private Transform originalLookAt;
    public Transform newTarget;


    void Update()
    {
        virtualCamera2.transform.position = sourceCameraTransform.position - (cameraController.virtualCamera.transform.position - sourceCameraTransform.position) * 0.5f;
        ChangeLookTarget(newTarget);

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (cameraController != null && cameraController.virtualCamera != null && sourceCameraTransform != null)
            {
                if (!isActive)
                {
                    virtualCamera2.transform.position = sourceCameraTransform.position - (cameraController.virtualCamera.transform.position - sourceCameraTransform.position) * 0.5f;
                    cameraController.virtualCamera.Priority = activePriority;
                    ChangeLookTarget(newTarget);
                    isActive = true;
                }
                else
                {
                    cameraController.virtualCamera.Priority = defaultPriority;
                    isActive = false;
                }
            }
        }
        if (cameraController != null) Debug.Log(cameraController.gameObject.name);
    }

    private void ChangeLookTarget(Transform target)
    {
        if (cameraController.virtualCamera != null && target != null)
        {
            virtualCamera2.LookAt = target;
        }
    }
}
