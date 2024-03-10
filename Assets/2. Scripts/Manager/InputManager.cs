using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private BuildingSpawner spawner;
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile"))) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                if (!tile.CheckBuilding()) return;

                spawner.SpawnBuilding(hit.transform);
            }
        }
    }
}
