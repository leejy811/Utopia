using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineManager : MonoBehaviour
{
    public int day;

    public void DailyUpdate()
    {

    }

    private int CalculateIncome()
    {
        int totalIncome = 0;

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            switch (building.type)
            {
                case BuildingType.Residential:
                    ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
                    totalIncome += residentialBuilding.residentCnt.cur * building.happinessRate * (4 - building.grade);
                    break;
                case BuildingType.Commercial:
                    CommercialBuilding commercialBuilding = building as CommercialBuilding;
                    totalIncome += commercialBuilding.income * building.happinessRate;
                    break;
                case BuildingType.culture:
                    CultureBilding cultureBilding = building as CultureBilding;
                    totalIncome += cultureBilding.income * building.happinessRate;
                    break;
            }
        }

        foreach (Building building in BuildingSpawner.instance.buildings)
        {
            switch (building.type)
            {
                case BuildingType.Residential:
                    ResidentialBuilding residentialBuilding = building as ResidentialBuilding;
                    totalIncome += residentialBuilding.residentCnt.cur * building.happinessRate * (4 - building.grade);
                    break;
                case BuildingType.Commercial:
                    CommercialBuilding commercialBuilding = building as CommercialBuilding;
                    totalIncome += commercialBuilding.income * building.happinessRate;
                    break;
                case BuildingType.culture:
                    CultureBilding cultureBilding = building as CultureBilding;
                    totalIncome += cultureBilding.income * building.happinessRate;
                    break;
            }
        }

        return totalIncome;
    }
}
