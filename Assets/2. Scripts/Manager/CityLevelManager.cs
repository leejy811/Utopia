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

public class CityLevelManager : MonoBehaviour
{
    public static CityLevelManager instance;

    public int levelIdx;
    public CityLevel[] level;

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
            LevelUp();
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
}
