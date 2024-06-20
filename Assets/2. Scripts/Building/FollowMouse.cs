using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowMouse : MonoBehaviour
{
    public GameObject tile;
    public GameObject redLine;
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
        {
            transform.position = tile.transform.position;
            redLine.SetActive(true);
        }
        else
        {
            SetPosition();
            redLine.SetActive(false);
        }
    }

    void SetPosition()
    {
        mousePos = Input.mousePosition;
        
        mousePos = main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, main.nearClipPlane + 2));
        transform.position = mousePos;
    }

    public void SetRedLineSize(int index)
    {
        float size = BuildingSpawner.instance.buildingPrefabs[index].GetComponent<Building>().influenceSize;
        redLine.transform.localScale = new Vector3(size * 2 + 1, 1, size * 2 + 1);
    }
}
