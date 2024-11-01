using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRacingCamera : MonoBehaviour
{
    public Transform targetHorse;
    public float mapWidth;
    public float centerX;

    private Camera cam;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        FollowTarget();
        LimitCamera();
    }

    private void FollowTarget()
    {
        if (targetHorse == null)
        {
            transform.position += Vector3.left * 100.0f;
            return;
        }

        transform.position = new Vector3(targetHorse.position.x, transform.position.y, transform.position.z);
    }

    private void LimitCamera()
    {
        float width = cam.orthographicSize * Screen.width / Screen.height;
        float xRange = mapWidth - width;
        float clampX = Mathf.Clamp(transform.position.x, centerX - xRange, centerX + xRange);
        transform.position = new Vector3(clampX, transform.position.y, transform.position.z);
    }
}
