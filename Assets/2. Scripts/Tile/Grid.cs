using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    static public Grid instance;

    public GameObject tilePrefab;
    public Tile[ , ] tiles;
    public int tileCost;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int startPoint;
    [SerializeField] private Vector2Int startTileSize;
    [SerializeField] private Vector3 cameraOffset;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SetTile();
        SetCamera();
    }

    private void SetTile()
    {
        if (width % 2 != 0) width++;
        if (height % 2 != 0) height++;

        tiles = new Tile[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(startPoint.x + i, 0, startPoint.y + j), tilePrefab.transform.rotation, transform);
                tiles[i, j] = tile.GetComponent<Tile>();
                if (i >= (width / 2) - (startTileSize.x / 2) && i < (width / 2) + (startTileSize.x / 2) &&
                    j >= (height / 2) - (startTileSize.y / 2) && j < (height / 2) + (startTileSize.y / 2))
                    tiles[i, j].SetTilePurchased(true);
                else
                    tiles[i, j].SetTilePurchased(false);
            }
        }
    }

    private void SetCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = ((Vector3Int)startPoint) + cameraOffset;
    }
}
