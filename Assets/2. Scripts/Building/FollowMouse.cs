using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowMouse : MonoBehaviour
{
    public GameObject tile;
    public bool isbool;
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
            transform.position = tile.transform.position + Vector3.up * 0.1f;
        else
            SetPosition();
    }

    void SetPosition()
    {
        mousePos = Input.mousePosition;
        
        mousePos = main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, main.nearClipPlane + 2));
        transform.position = mousePos;
    }
}
