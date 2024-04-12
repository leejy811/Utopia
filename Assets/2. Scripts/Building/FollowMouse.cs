using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowMouse : MonoBehaviour
{
    public GameObject tile;
    Vector3 mousePos;
    Camera main;

    void Start()
    {
        main = Camera.main;
        SetPosition();
    }

    void Update()
    {
        if (tile != null)
            transform.position = tile.transform.position;
        else
            SetPosition();
    }

    void SetPosition()
    {
        mousePos = Input.mousePosition;
        
        mousePos = main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, main.nearClipPlane + 1));
        transform.position = mousePos;
    }
}
