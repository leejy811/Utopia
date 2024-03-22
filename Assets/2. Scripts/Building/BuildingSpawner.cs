using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    static public BuildingSpawner instance;

    public List<Building> buildings;

    public GameObject[] buildingPrefabs;

    public int[] buildingCount = new int[8];

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PlaceBuilding(int index, Transform spawnTrans)
    {
        Building building = Instantiate(buildingPrefabs[index], new Vector3(spawnTrans.position.x, 0, spawnTrans.position.z), Quaternion.identity, transform).GetComponent<Building>();
        spawnTrans.gameObject.GetComponent<Tile>().building = building.gameObject;
        spawnTrans.gameObject.GetComponent<Tile>().ApplyInfluenceToBuilding();
        building.SetPosition(spawnTrans.position);

        buildingCount[(int)building.subType + 1]++;
        building.count = buildingCount[(int)building.subType + 1];

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
    }

    public void RemoveBuilding(GameObject building)
    {
        for(int i = 0;i < buildings.Count; i++)
        {
            if (building.GetComponent<Building>().position == buildings[i].position)
            {
                buildings.RemoveAt(i);
                break;
            }
        }
    }
}
