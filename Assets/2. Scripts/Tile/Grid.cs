using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour, IObserver
{
    static public Grid instance;

    [Header("Tile")]
    public GameObject tilePrefab;
    public Tile[ , ] tiles;
    public Tile[ , ] levelUpTiles;
    public int[] tileCost;
    public int[] tileCostPerDay;

    public Color[] tileColors;
    public Color[] tilePurchaseColors;

    public bool isColorMode;

    public int width;
    public int height;
    [SerializeField] private Vector2Int startPoint;
    public Vector2Int levelUPStartPoint;
    [SerializeField] private Vector2Int startTileSize;
    [SerializeField] private Vector3 cameraOffset;

    private bool isSetTile = false;

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
        int maxLevelIdx = CityLevelManager.instance.level.Length - 1;
        width = CityLevelManager.instance.level[maxLevelIdx].tileSize.x;
        height = CityLevelManager.instance.level[maxLevelIdx].tileSize.y;

        if (width % 2 != 0) width++;
        if (height % 2 != 0) height++;

        tiles = new Tile[width, height];
        levelUpTiles = new Tile[width, height];
        startTileSize = CityLevelManager.instance.level[0].tileSize;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CreateTile(tiles, startPoint, i, j);
                CreateTile(levelUpTiles, levelUPStartPoint, i, j);
            }
        }

        isSetTile = true;
    }

    private void CreateTile(Tile[,] inputTiles, Vector2Int startPos, int i, int j)
    {
        GameObject tile = Instantiate(tilePrefab, new Vector3(startPos.x + i, 0, startPos.y + j), tilePrefab.transform.rotation, transform);
        inputTiles[i, j] = tile.GetComponent<Tile>();
        if (i >= (width / 2) - (startTileSize.x / 2) && i < (width / 2) + (startTileSize.x / 2) &&
            j >= (height / 2) - (startTileSize.y / 2) && j < (height / 2) + (startTileSize.y / 2))
            inputTiles[i, j].SetTilePurchased(true);
        else
            inputTiles[i, j].SetTilePurchased(false);

        inputTiles[i, j].tilePos = new Vector2(i, j);
    }

    private void SetCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = ((Vector3Int)startPoint) + cameraOffset;
    }

    public void PurchaseTile(Vector2Int purchaseSize, Vector2Int prevSize, float time = 0.0f)
    {
        for (int i = (width / 2) - (purchaseSize.x / 2); i < (width / 2) + (purchaseSize.x / 2); i++)
        {
            for (int j = (height / 2) - (purchaseSize.y / 2); j < (height / 2) + (purchaseSize.y / 2); j++)
            {
                if (i > (width / 2) - (prevSize.x / 2) && i < (width / 2) + (prevSize.x / 2)
                    && j > (height / 2) - (prevSize.y / 2) && j < (height / 2) + (prevSize.y / 2))
                    continue;

                tiles[i, j].SetTilePurchased(true, time);
                levelUpTiles[i, j].SetTilePurchased(true, time);
            }
        }
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
    }

    public void Notify(EventState state)
    {
        if (state == EventState.TileColor && !isColorMode)
        {
            isColorMode = true;
            SetTileColorMode(true);
        }
        else if (isSetTile)
        {
            isColorMode = false;
            SetTileColorMode(false);
        }
    }

    public void PlaceTile(int index, Transform spawnTrans)
    {
        Vector3Int pos = new Vector3Int((int)spawnTrans.position.x, (int)spawnTrans.position.y, (int)spawnTrans.position.z);
        tiles[pos.x, pos.z].ChangeTileModel(index);

        for (int i = pos.x - 1; i <= pos.x + 1; i++)
        {
            for (int j = pos.z - 1; j <= pos.z + 1; j++)
            {
                if (i == pos.x && j == pos.z) continue;
                tiles[i, j].ReplaceTile();
            }
        }
    }

    public bool[] GetRoadInformation(Transform trans)
    {
        bool[] isRoad = new bool[4];

        Vector3Int pos = new Vector3Int((int)trans.position.x, (int)trans.position.y, (int)trans.position.z);
        isRoad[0] = pos.z + 1 < height ? tiles[pos.x, pos.z + 1].type == TileType.Road : false;
        isRoad[1] = pos.x + 1 < width ? tiles[pos.x + 1, pos.z].type == TileType.Road : false;
        isRoad[2] = pos.z - 1 >= 0 ? tiles[pos.x, pos.z - 1].type == TileType.Road : false;
        isRoad[3] = pos.x - 1 >= 0 ? tiles[pos.x - 1, pos.z].type == TileType.Road : false;

        return isRoad;
    }

    public bool[] GetWaterInformation(Transform trans)
    {
        bool[] isWater = new bool[8];

        Vector3Int pos = new Vector3Int((int)trans.position.x, (int)trans.position.y, (int)trans.position.z);

        int k = 0;
        for(int i = pos.x - 1;i <= pos.x + 1; i++)
        {
            for (int j = pos.z - 1; j <= pos.z + 1; j++)
            {
                if (i == pos.x && j == pos.z) continue;
                if (i < height && j < width && i >= 0 && j >= 0)
                    isWater[k] = tiles[i, j].type == TileType.Water;
                else
                    isWater[k] = false; 
                k++;
            }
        }

        return isWater;
    }
}
