using System.Collections;
using UnityEngine;
using Cinemachine;

public class LobbyRotateCamera : MonoBehaviour
{
    public float rotationSpeed = 90.0f;

    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        float direction = -1;

        Ray ray = new Ray(virtualCamera.transform.position, virtualCamera.transform.forward);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            virtualCamera.transform.RotateAround(hitPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
        }
    }
}