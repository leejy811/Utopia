using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public bool isBuyBuilding = false;
    public Building curPickBuilding;

    [SerializeField] private GameObject[] buildingPrefabs;

    public void SpawnBuilding(Transform spawnPos)
    {
        if (!isBuyBuilding) return;

        curPickBuilding.transform.position = new Vector3(spawnPos.position.x, 0, spawnPos.position.z);
        curPickBuilding.isSpawn = true;
        isBuyBuilding = false;
        curPickBuilding = null;
    }

    public void BuyBuilding(int index)
    {
        if (isBuyBuilding) return;
        isBuyBuilding = true;

        curPickBuilding = Instantiate(buildingPrefabs[index]).GetComponent<Building>();
    }
}
