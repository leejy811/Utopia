using UnityEngine;
using Cinemachine;

public class CameraPriorityController : MonoBehaviour
{
    public CinemachineCameraController cameraController;
    public Transform sourceCameraTransform;
    public CinemachineVirtualCamera virtualCamera2;

    private int defaultPriority = 5;
    private int activePriority = 20;
    private Transform originalLookAt;
    public Transform newTarget;

    public bool isActive { get; set; } = true;
    public bool isDelayActive = false;
    private float debounceTime = 3f;
    private float lastToggleTime;

    void Update()
    {
        if (cameraController.virtualCamera.Priority == 20)
        {
            if (isDelayActive && Time.time > lastToggleTime + debounceTime)
            {
                virtualCamera2.transform.position = sourceCameraTransform.position - (cameraController.virtualCamera.transform.position - sourceCameraTransform.position) * 0.5f;
                ChangeLookTarget(newTarget);
                isDelayActive = false;
            }
            else if (!isDelayActive)
            {
                virtualCamera2.transform.position = sourceCameraTransform.position - (cameraController.virtualCamera.transform.position - sourceCameraTransform.position) * 0.5f;
                ChangeLookTarget(newTarget);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                cameraController.virtualCamera.Priority = defaultPriority;
                isActive = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                virtualCamera2.LookAt = null;
                float direction = Input.GetKey(KeyCode.Q) ? 1 : -1;

                Ray ray = new Ray(virtualCamera2.transform.position, virtualCamera2.transform.forward);
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                float enter = 0.0f;

                if (plane.Raycast(ray, out enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    virtualCamera2.transform.RotateAround(hitPoint, Vector3.up, 90f * direction * Time.deltaTime);
                }
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                lastToggleTime = Time.time;
                cameraController.virtualCamera.Priority = activePriority;
                isDelayActive = true;
                isActive = true;
            }
        }
    }

    public void ChangeLookTarget(Transform target)
    {
        if (cameraController.virtualCamera != null && target != null)
        {
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

    void RotateCamera_VC2()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            float direction = Input.GetKey(KeyCode.Q) ? 1 : -1;
            float rotationSpeed = 90.0f;

            Vector3 pivotPoint = new Vector3(newTarget.transform.position.x, newTarget.transform.position.y, newTarget.transform.position.z);

            virtualCamera2.transform.RotateAround(pivotPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
        }
    }
}
