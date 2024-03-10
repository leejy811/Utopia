using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    static public BuildingSpawner instance;

    public bool isBuyBuilding = false;
    public Building curPickBuilding;
    public List<Building> buildings;

    [SerializeField] private GameObject[] buildingPrefabs;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SpawnBuilding(Transform spawnPos)
    {
        if (!isBuyBuilding) return;

        buildings.Add(curPickBuilding);
        curPickBuilding.transform.position = new Vector3(spawnPos.position.x, 0, spawnPos.position.z);
        curPickBuilding.isSpawn = true;
        isBuyBuilding = false;
        curPickBuilding = null;
    }

    public void BuyBuilding(int index)
    {
        if (isBuyBuilding) return;
        isBuyBuilding = true;

        curPickBuilding = Instantiate(buildingPrefabs[index], transform).GetComponent<Building>();
    }
}
