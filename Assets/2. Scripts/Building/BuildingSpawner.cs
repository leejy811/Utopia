using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    static public BuildingSpawner instance;

    public List<Building> buildings;

    public GameObject[] buildingPrefabs;

    public int[] buildingCount;

    public bool isHighlightMode;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        buildingCount = new int[buildingPrefabs.Length];
    }

    public void PlaceBuilding(int index, Transform spawnTrans)
    {
        Building building = Instantiate(buildingPrefabs[index], new Vector3(spawnTrans.position.x, 0, spawnTrans.position.z), spawnTrans.rotation, transform).GetComponent<Building>();
        spawnTrans.gameObject.GetComponent<Tile>().building = building.gameObject;
        spawnTrans.gameObject.GetComponent<Tile>().ApplyInfluenceToBuilding();
        spawnTrans.gameObject.GetComponent<Tile>().Coloring(Grid.instance.isColorMode);
        building.SetPosition(spawnTrans.position);

        buildingCount[index]++;
        building.count = buildingCount[index];

        RoutineManager.instance.SetCityHappiness(building.happinessRate, 1);

        switch (building.type)
        {
            case BuildingType.Residential:
                buildings.Add(building as ResidentialBuilding);
                break;
            case BuildingType.Commercial:
                buildings.Add(building as CommercialBuilding);
                break;
            case BuildingType.Culture:
                buildings.Add(building as CultureBuilding);
                break;
            case BuildingType.Service:
                buildings.Add(building as ServiceBuilding);
                break;
        }

        BottonSound.instance.Click("Play_buildingsound");
    }

    public void RemoveBuilding(GameObject building)
    {
        for (int i = 0;i < buildings.Count; i++)
        {
            if (building.GetComponent<Building>().position == buildings[i].position)
            {
                RoutineManager.instance.SetCityHappiness(buildings[i].happinessRate * -1, -1);
                buildings.RemoveAt(i);
                break;
            }
        }
    }

    public void ChangeViewState(ViewStateType stateType)
    {
        foreach(Building building in buildings)
        {
            building.ChangeViewState(stateType);
        }
    }

    public void EventBuildingsHighlight()
    {
        isHighlightMode = !isHighlightMode;

        foreach (Building building in buildings)
        {
            if (building.GetEventProblemCount() > 0 && isHighlightMode)
                ShopManager.instance.SetObjectColor(building.gameObject, Color.yellow);
            else
                ShopManager.instance.SetObjectColor(building.gameObject, Color.white);
        }
    }

    public int GetEventBuildingCount()
    {
        int cnt = 0;

        foreach (Building building in buildings)
        {
            if (building.GetEventProblemCount() > 0)
                cnt++;
        }

        return cnt;
    }
}
