using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f; 
    public float rotationSpeed = 90.0f; 
    public float zoomSpeed = 5.0f;

    public float minX = -2f;
    public float maxX = 2f;
    public float minZ = -2f;
    public float maxZ = 2f;

    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = (h * right + v * forward) * moveSpeed * Time.deltaTime;

        float newX = Mathf.Clamp(transform.position.x + moveDirection.x, minX, maxX);
        float newZ = Mathf.Clamp(transform.position.z + moveDirection.z, minZ, maxZ);

        transform.position = new Vector3(newX, transform.position.y, newZ);


        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            float direction = Input.GetKey(KeyCode.Q) ? 1 : -1;

            Ray ray = new Ray(transform.position, transform.forward);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float enter = 0.0f;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.RotateAround(hitPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
            }
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - zoom, 1f); 
    }
}