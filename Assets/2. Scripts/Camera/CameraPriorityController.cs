using UnityEngine;
using Cinemachine;

public class CameraPriorityController : MonoBehaviour
{
    public CinemachineCameraController cameraController;
    public Transform sourceCameraTransform;
    public CinemachineVirtualCamera virtualCamera2;

    private int defaultPriority = 5;
    private int activePriority = 20;
    public Transform newTarget;

    public bool isActive { get; set; } = true;

    public void ChangeLookTarget(Transform target)
    {
        if (cameraController.virtualCamera != null && target != null)
        {
            virtualCamera2.transform.position = sourceCameraTransform.position - (cameraController.virtualCamera.transform.position - sourceCameraTransform.position) * 0.5f;
            virtualCamera2.LookAt = target;
        }
    }

    public void ChangeActiveState()
    {
        if (cameraController != null && cameraController.virtualCamera != null && sourceCameraTransform != null)
        {
            if (!isActive)
            {
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
}
