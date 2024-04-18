using UnityEngine;
using Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 90.0f;
    public float zoomSpeed = 5.0f;

    public float minX = -40f;
    public float maxX = 40f;
    public float minZ = -40f;
    public float maxZ = 40f;

    private CinemachineTransposer transposer;
    private CinemachineComposer composer;
    private bool isWithinBounds = true;

    private Vector3 originalPosition;
    private Transform originalLookAt;
    private bool isUsingOriginalTarget = true;
    public Transform newTarget;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    }

    void Update()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleCameraTarget();
        }


    }

    void MoveCamera()
    {
        Vector3 forward = virtualCamera.transform.forward;
        Vector3 right = virtualCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = (h * right + v * forward) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = virtualCamera.transform.position + moveDirection;

        if (isWithinBounds)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        }

        if (newPosition.x >= minX && newPosition.x <= maxX && newPosition.z >= minZ && newPosition.z <= maxZ)
        {
            isWithinBounds = true;
        }
        else
        {
            isWithinBounds = false;
        }

        virtualCamera.transform.position = newPosition;
    }

    void RotateCamera()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            float direction = Input.GetKey(KeyCode.Q) ? 1 : -1;

            Ray ray = new Ray(virtualCamera.transform.position, virtualCamera.transform.forward);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float enter = 0.0f;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                virtualCamera.transform.RotateAround(hitPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
            }

            isWithinBounds = false; 
        }
    }


    void ZoomCamera()
    {
        if (virtualCamera.m_Lens.Orthographic)
        {
            float zoomChange = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Max(virtualCamera.m_Lens.OrthographicSize - zoomChange, 1f);
        }
    }

    void ToggleCameraTarget()
    {
        if (isUsingOriginalTarget)
        {
            if (newTarget != null)
            {
                ChangeAimTarget(newTarget);
                isUsingOriginalTarget = false;
            }
        }
        else
        {
            ChangeAimTarget(originalLookAt);
            virtualCamera.transform.position = originalPosition;
            isUsingOriginalTarget = true;
        }
    }

    void ChangeAimTarget(Transform newTarget)
    {
        if (virtualCamera != null)
        {
            virtualCamera.LookAt = newTarget;
        }
    }
}
