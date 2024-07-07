using System.Collections;
using UnityEngine;
using Cinemachine;

public class CinemachineCameraController_4 : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera virtualCamera_2;
    public float rotationSpeed = 90.0f;

    public Coroutine rotateOnCoroutine;

    Vector3 Position_1 = new Vector3(6f, 9.4f, 6f);
    Vector3 Position_2 = new Vector3(1006f, 9.4f, 6f);

    public IEnumerator SetCameraPrioritiesWithDelay()
    {
        virtualCamera_2.Priority = 100;
        yield return new WaitForSeconds(2.0f);
        virtualCamera_2.transform.position = Position_2;
        rotateOnCoroutine = StartCoroutine(RotateCameraOn());
    }

    public IEnumerator RotateCameraOn()
    {
        while (true)
        {
            RotateCamera();
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator RotateCameraOff()
    {
        while (true)
        {
            RotateCamera_2();
            float currentYRotation = virtualCamera_2.transform.eulerAngles.y;
            if (IsNearTargetAngle(currentYRotation, 45))
            {
                virtualCamera_2.transform.position = Position_1;
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        virtualCamera_2.Priority = 10;
    }

    void RotateCamera()
    {

        float direction = -1;

        Ray ray = new Ray(virtualCamera_2.transform.position, virtualCamera_2.transform.forward);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            virtualCamera_2.transform.RotateAround(hitPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
        }
    }

    void RotateCamera_2()
    {
        float direction = -1;

        Ray ray = new Ray(virtualCamera_2.transform.position, virtualCamera_2.transform.forward);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            float currentYRotation = virtualCamera_2.transform.eulerAngles.y;

            if (!IsNearTargetAngle(currentYRotation, 35) && !IsNearTargetAngle(currentYRotation, 45) && !IsNearTargetAngle(currentYRotation, 0))
            {
                virtualCamera_2.transform.RotateAround(hitPoint, Vector3.up, rotationSpeed * direction * Time.deltaTime);
            }
        }
    }

    bool IsNearTargetAngle(float currentAngle, float targetAngle)
    {
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        return angleDifference < 1.0f;
    }

}