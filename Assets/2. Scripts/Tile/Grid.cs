using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    static public Grid instance;

    public GameObject tilePrefab;
    public Tile[ , ] tiles;
    public int tileCost;

    public Color[] tileColors;

    public bool isColorMode;
    public bool isInfluenceMode;
    public bool isAddtionalMode;

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

    private void SetTileColorMode(bool isOn)
    {
        Color color;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].building != null)
                {
                    Building building = tiles[i, j].building.GetComponent<Building>();
                    color = isOn ? tileColors[(int)building.type] : Color.green;

                    tiles[i, j].SetTileColor(color);
                    building.ChangeViewState((ViewStateType)(Convert.ToInt32(!isOn) * 2));
                }
            }
        }
    }

    public void SetTileColorMode()
    {
        if(isInfluenceMode)
        {
            isColorMode = false;
            SetTileColorMode(isAddtionalMode);
            return;
        }

        isColorMode = !isColorMode;
        SetTileColorMode(isColorMode);

        if (isColorMode)
            SetTileInfluenceMode();
    }

    public void SetTileInfluenceMode()
    {
        isInfluenceMode = !isInfluenceMode;

        if (isInfluenceMode)
            SetTileColorMode();
    }

    public void AddMode()
    {
        isAddtionalMode = !isAddtionalMode;

        if (isInfluenceMode)
            SetTileColorMode();
    }

    public void NotifyTileInfluence(Transform tileTransform)
    {
        Tile tile = tiles[(int)tileTransform.position.x, (int)tileTransform.position.y];
        UIManager.instance.SetTileInfluencePopUp(tile);
    }
}
