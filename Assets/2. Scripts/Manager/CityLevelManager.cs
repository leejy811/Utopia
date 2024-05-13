using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CityType { Village = 0, Town, City, Utopia }

[System.Serializable]
public struct CityLevel
{
    public CityType type;
    public int residentCondition;
    public int happinessCondition;
    public int moneyReward;
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

    public void UpdateCityType()
    {
        if(levelIdx == level.Length - 1)
            return;

        if(ResidentialBuilding.cityResident >= level[levelIdx + 1].residentCondition &&
            RoutineManager.instance.cityHappiness >= level[levelIdx + 1].happinessCondition)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        levelIdx++;
        ShopManager.instance.GetMoney(level[levelIdx].moneyReward);

        if (levelIdx == level.Length)
            EventManager.instance.eventProb = 0.5f;

        UIManager.instance.SetCityLevelUpPopUp(levelIdx);
    }

    public bool CheckBuildingLevel(Building building)
    {
        int levelCondition = 3 - levelIdx;
        return building.grade >= levelCondition;
    }
}
