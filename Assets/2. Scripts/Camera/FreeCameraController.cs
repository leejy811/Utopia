using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f; 
    public float rotationSpeed = 100.0f; 
    public float zoomSpeed = 2.0f; 
    public float panSpeed = 5.0f; 

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
        }

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        if (Input.GetMouseButton(2)) 
        {
            Vector3 move = new Vector3(-Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime, -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime, 0);
            transform.Translate(move, Space.World);
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        transform.position += transform.forward * zoom;
    }
}
