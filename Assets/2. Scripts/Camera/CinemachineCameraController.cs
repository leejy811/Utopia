using UnityEngine;
using Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 90.0f;
    public float zoomSpeed = 5.0f;
    public float smoothZoomSpeed = 2.0f;
    private float targetOrthographicSize= 6.2f;


    public float minX = -10f;
    public float maxX = 25f;
    public float minZ = -10f;
    public float maxZ = 25f;

    public CinemachineTransposer transposer;
    public CinemachineComposer composer;
    private bool isWithinBounds = true;

    private Vector3 originalPosition;
    private bool isUsingOriginalTarget = true;
    public float publicOrthographicSize;

    public GameObject realCamera;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();

        publicOrthographicSize = 6.2f;
        virtualCamera.m_Lens.OrthographicSize = publicOrthographicSize;
    }

    void Update()
    {
        if (!InputManager.canInput) return;

        if (virtualCamera.Priority == 20)
        {
            MoveCamera();
            RotateCamera();
        }
        ZoomCamera();
        PitchCamera();
        UpdateCameraSettings();
        AkSoundEngine.SetRTPCValue("ZOOM", publicOrthographicSize);
        AkSoundEngine.SetRTPCValue("DIST", ShopManager.instance.GetDistToTarget(realCamera.transform));
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

        Vector3 expectedPosition = transform.position + moveDirection;

        Ray ray = new Ray(virtualCamera.transform.position, virtualCamera.transform.forward);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            bool insideX = hitPoint.x >= 0f && hitPoint.x <= 32f;
            bool insideZ = hitPoint.z >= 0f && hitPoint.z <= 32f;

            if (insideX && insideZ)
            {
                transform.Translate(moveDirection, Space.World);
            }
            else if (!insideX && insideZ)
            {
                if ((hitPoint.x <= 0f && h >= 0) || (hitPoint.x >= 32f && h <= 0))
                {
                    transform.Translate(Vector3.right * h * moveSpeed * Time.deltaTime, Space.World);
                    transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime, Space.World);
                }
            }
            else if (insideX && !insideZ)
            {
                if ((hitPoint.z <= 0f && v >= 0) || (hitPoint.z >= 32f && v <= 0))
                {
                    transform.Translate(Vector3.right * h * moveSpeed * Time.deltaTime, Space.World);
                    transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime, Space.World);
                }
            }
            else if (!insideX && !insideZ)
            {
                if ((hitPoint.z <= 0f && v >= 0 && hitPoint.x <= 0f && h >= 0) ||
                    (hitPoint.z <= 0f && v >= 0 && hitPoint.x >= 0f && h <= 0) ||
                    (hitPoint.z >= 0f && v <= 0 && hitPoint.x <= 0f && h >= 0) ||
                    (hitPoint.z >= 0f && v <= 0 && hitPoint.x >= 0f && h <= 0))
                {
                    transform.Translate(Vector3.right * h * moveSpeed * Time.deltaTime, Space.World);
                    transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime, Space.World);
                }
            }
        }
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
            targetOrthographicSize -= zoomChange;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, 1f, 25f);

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * smoothZoomSpeed);

            realCamera.transform.localPosition = new Vector3(0, 0, 15 - virtualCamera.m_Lens.OrthographicSize);
            UpdateMoveSpeed();
        }
    }


    void UpdateMoveSpeed()
    {
        if (virtualCamera.m_Lens.OrthographicSize <= 5)
        {
            moveSpeed = 5.0f;
        }
        else
        {
            moveSpeed = virtualCamera.m_Lens.OrthographicSize * 2;
        }
    }

    void PitchCamera()
    {
        if (Input.GetMouseButton(2))
        {
            float pitchChange = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            virtualCamera.transform.Rotate(-pitchChange, 0f, 0f);

            float currentXRotation = virtualCamera.transform.localEulerAngles.x;
            currentXRotation = (currentXRotation > 180) ? currentXRotation - 360 : currentXRotation;
            currentXRotation = Mathf.Clamp(currentXRotation, -80, 80);
            virtualCamera.transform.localEulerAngles = new Vector3(currentXRotation, virtualCamera.transform.localEulerAngles.y, 0);
        }
    }

    void UpdateCameraSettings()
    {
        publicOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
    }
}
