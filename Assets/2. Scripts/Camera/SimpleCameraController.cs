using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection, Space.World);
    }
}