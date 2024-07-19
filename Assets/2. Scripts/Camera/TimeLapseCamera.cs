using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLapseCamera : MonoBehaviour
{
    public float rotationSpeed = 90.0f;
    public float[] zoomSizes;

    public Coroutine rotateOnCoroutine;

    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator SetCameraPrioritiesWithDelay()
    {
        virtualCamera.m_Lens.OrthographicSize = zoomSizes[CityLevelManager.instance.levelIdx];
        virtualCamera.Priority = 100;
        yield return new WaitForSeconds(2.0f);
        virtualCamera.transform.position += new Vector3(1000, 0, 0);
    }

    public IEnumerator RotateCameraOn()
    {
        while (true)
        {
            RotateCamera();
            yield return new WaitForFixedUpdate();
        }
    }

    public void RotateCameraOff()
    {
        virtualCamera.transform.position -= new Vector3(1000, 0, 0);
        virtualCamera.Priority = 1;
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
