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
        virtualCamera.transform.RotateAround(virtualCamera.LookAt.position, Vector3.up, rotationSpeed * direction * Time.deltaTime);
    }
}