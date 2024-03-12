using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    static public BuildingSpawner instance;

    public List<Building> buildings;

    public GameObject[] buildingPrefabs;

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
        Tile tile = spawnTrans.gameObject.GetComponent<Tile>();
        if (!tile.CheckBuilding()) return;

        tile.existBuilding = true;
        Building building = Instantiate(buildingPrefabs[index], new Vector3(spawnTrans.position.x, 0, spawnTrans.position.z), Quaternion.identity, transform).GetComponent<Building>();
        buildings.Add(building);
    }
}
