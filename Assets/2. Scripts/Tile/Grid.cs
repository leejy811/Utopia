using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject tilePrefab;
    public Tile[ , ] tiles;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int startPoint;
    [SerializeField] private Vector3 cameraOffset;

    private void Awake()
    {
        SetTile();
        SetCamera();
    }

    private void SetTile()
    {
        tiles = new Tile[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(startPoint.x + i, 0, startPoint.y + j), Quaternion.identity, transform);
                tiles[i, j] = tile.GetComponent<Tile>();
            }
        }
    }

    private void SetCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = ((Vector3Int)startPoint) + cameraOffset;
    }
}
