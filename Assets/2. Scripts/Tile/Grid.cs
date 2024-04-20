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
    public Color[] tilePurchaseColors;

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

    public void SetTileColorMode(bool isOn)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].building != null)
                {
                    tiles[i, j].Coloring(isOn);
                }
            }
        }
    }

    public void SetTileColorMode()
    {
        isColorMode = !isColorMode;
        SetTileColorMode(isColorMode);

        if (isAddtionalMode && !isColorMode)
        {
            isAddtionalMode = false;
            SetTileInfluenceMode(false);
        }

        if (isColorMode && isInfluenceMode)
            SetTileInfluenceMode();
    }

    public void SetTileInfluenceMode()
    {
        isInfluenceMode = !isInfluenceMode;

        SetTileInfluenceMode(isInfluenceMode);

        if (isAddtionalMode && !isInfluenceMode)
        {
            isAddtionalMode = false;
            SetTileColorMode(false);
        }

        if (isInfluenceMode & isColorMode)
            SetTileColorMode();
    }

    public void SetTileInfluenceMode(bool isOn)
    {
        if (!isOn)
        {
            UIManager.instance.SetTileInfluencePopUp(null);
        }
    }

    public void AddMode()
    {
        isAddtionalMode = !isAddtionalMode;

        if (isInfluenceMode)
        {
            if (isAddtionalMode)
                SetTileColorMode(true);
            else
                SetTileColorMode(false);
        }
        else if (isColorMode)
        {
            if (isAddtionalMode)
                SetTileInfluenceMode(true);
            else
                SetTileInfluenceMode(false);
        }
    }

    public void NotifyTileInfluence(Transform tileTransform)
    {
        Tile tile = tiles[(int)tileTransform.position.x, (int)tileTransform.position.z];
        UIManager.instance.SetTileInfluencePopUp(tile);
    }
}
