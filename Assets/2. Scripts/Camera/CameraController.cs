using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float moveSpeed = 10f; 
    public float zoomSpeed = 10f; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(-90);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(90);
        }

        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        virtualCamera.transform.position += moveVector * moveSpeed * Time.deltaTime;

        if (virtualCamera.TryGetComponent(out CinemachineVirtualCamera vcam))
        {
            vcam.m_Lens.FieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            vcam.m_Lens.FieldOfView = Mathf.Clamp(vcam.m_Lens.FieldOfView, 20, 120); 
        }
    }

    private void RotateCamera(float angle)
    {
        var rotation = virtualCamera.transform.rotation.eulerAngles;
        rotation.y += angle;
        virtualCamera.transform.rotation = Quaternion.Euler(rotation);
    }
}
