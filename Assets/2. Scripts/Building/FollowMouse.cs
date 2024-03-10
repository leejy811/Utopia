using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector3 mousePos;
    Camera main;
    Building building;

    void Start()
    {
        building = gameObject.GetComponent<Building>();
        main = Camera.main;
        SetPosition();
    }

    void Update()
    {
        if (building.isSpawn) return;
        SetPosition();
    }

    void SetPosition()
    {
        mousePos = Input.mousePosition;
        mousePos = main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -main.transform.position.z));
        transform.position = mousePos;
    }
}
