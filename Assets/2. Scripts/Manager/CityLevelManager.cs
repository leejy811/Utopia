using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CityType { Village = 0, Town, City, Utopia }

[System.Serializable]
public struct CityLevel
{
    public CityType type;
    public int debtWeek;
    public Vector2Int tileSize;
}

[System.Serializable]
public struct FrameInfo
{
    public int index;
    public Vector2Int position;
    public Quaternion rotation;
    public bool isInsert;

    public FrameInfo(int idx, Vector2Int pos, Quaternion rot, bool isIn)
    {
        index = idx;
        position = pos;
        rotation = rot;
        isInsert = isIn;
    }
}

public class CityLevelManager : MonoBehaviour
{
    public static CityLevelManager instance;

    public int levelIdx;
    public CityLevel[] level;
    public Queue<FrameInfo> frames = new Queue<FrameInfo>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void UpdateCityType(int week)
    {
        if(levelIdx == level.Length - 1)
            return;

        for (int i = 0;i <= levelIdx;i++)
        {
            week -= level[i].debtWeek;
        }

        if (week >= level[levelIdx + 1].debtWeek)
            StartCoroutine(PlayTimeLapse());
    }

    private void LevelUp()
    {
        levelIdx++;
        Grid.instance.PurchaseTile(level[levelIdx].tileSize);
        UIManager.instance.notifyObserver(EventState.CityLevelUp);
        UIManager.instance.SetCityLevelUp();
    }

    public bool CheckBuildingLevel(Building building)
    {
        return building.grade <= levelIdx;
    }

    IEnumerator PlayTimeLapse()
    {
        while(frames.Count > 0)
        {
            FrameInfo curFrame = frames.Dequeue();

            if (curFrame.isInsert)
            {
                GameObject prefab = BuildingSpawner.instance.buildingPrefabs[curFrame.index];
                Vector3 startPos = new Vector3(Grid.instance.levelUPStartPoint.x, 0, Grid.instance.levelUPStartPoint.y);
                Vector3 spawnPos = new Vector3(curFrame.position.x, 0, curFrame.position.y);
                GameObject building = Instantiate(prefab, spawnPos + startPos, curFrame.rotation, transform);
                Grid.instance.levelUpTiles[curFrame.position.x, curFrame.position.y].building = building;
            }
            else
            {
                Destroy(Grid.instance.levelUpTiles[curFrame.position.x, curFrame.position.y].building);
            }

            yield return new WaitForSeconds(0.5f);
        }

        LevelUp();
    }
}
