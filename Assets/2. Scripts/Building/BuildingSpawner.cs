using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingSpawner : MonoBehaviour, IObserver
{
    static public BuildingSpawner instance;

    public List<Building> buildings;
    public List<EnterBuilding> gameBuildings;

    public GameObject[] buildingPrefabs;

    public int[] buildingCount;
    public int[] randomParameter;
    public int[,] buildingGradeCount;
    public int buildingRemoveCount;

    private bool isHighlight;
    private List<TemporayUI> eventIconTemps = new List<TemporayUI>();

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
        buildingCount = new int[buildingPrefabs.Length];
        randomParameter = new int[buildingPrefabs.Length];
        for (int i = 0; i < randomParameter.Length; i++)
        {
            randomParameter[i] = Random.Range(-1, 2);
        }

        buildingGradeCount = new int[System.Enum.GetValues(typeof(BuildingType)).Length, CityLevelManager.instance.level.Length - 1];
    }

    public void PlaceBuilding(int index, Transform spawnTrans)
    {
        Building building = Instantiate(buildingPrefabs[index], new Vector3(spawnTrans.position.x, 0, spawnTrans.position.z), spawnTrans.rotation, transform).GetComponent<Building>();

        if (building.type != BuildingType.Residential)
        {
            (building as UtilityBuilding).SetParameter(randomParameter[index]);
            randomParameter[index] = Random.Range(-1, 2);
        }

        spawnTrans.gameObject.GetComponent<Tile>().building = building.gameObject;
        spawnTrans.gameObject.GetComponent<Tile>().ApplyInfluenceToBuilding();
        spawnTrans.gameObject.GetComponent<Tile>().Coloring(Grid.instance.isColorMode);
        building.SetPosition(spawnTrans.position);
        building.index = index;
        CityLevelManager.instance.curFrames.Enqueue(new FrameInfo() { index = index, position = new Vector2Int((int)spawnTrans.position.x, (int)spawnTrans.position.z), rotation = spawnTrans.rotation, isInsert = true });

        buildingCount[index]++;
        building.count = buildingCount[index];
        buildingGradeCount[(int)building.type, building.grade]++;

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
                if (GameManager.instance.curMapType == MapType.Utopia)
                    buildings.Add(building as CultureBuilding);
                else if (GameManager.instance.curMapType == MapType.Totopia)
                {
                    buildings.Add(building as EnterBuilding);
                    gameBuildings.Add(building as EnterBuilding);
                }
                break;
            case BuildingType.Service:
                buildings.Add(building as ServiceBuilding);
                break;
        }

        SoundManager.instance.PostEvent("Play_buildingsound");

        if(buildings.Count >= 1)
            AkSoundEngine.PostEvent("Stop_construction", gameObject);
    }

    public void LoadBuilding(BuildingData data)
    {
        Building building = Instantiate(buildingPrefabs[data.index], new Vector3(data.position.x, 0, data.position.y), data.rotation, transform).GetComponent<Building>();

        if (building.type == BuildingType.Residential)
        {
            BoundaryValue value = building.values[ValueType.Resident];
            value.cur = data.parameter;
            building.values[ValueType.Resident] = value;
        }
        else if (GameManager.instance.curMapType == MapType.Totopia && building.type == BuildingType.Culture)
        {
            BoundaryValue value = building.values[ValueType.betChip];
            value.cur = data.parameter;
            building.values[ValueType.betChip] = value;
            (building as EnterBuilding).betTimes = (int)data.extraParameter;
        }
        else
        {
            BoundaryValue value = building.values[ValueType.utility];
            value.cur = data.parameter;
            building.values[ValueType.utility] = value;
        }

        Tile tile = Grid.instance.tiles[data.position.x, data.position.y];
        tile.building = building.gameObject;
        tile.ApplyInfluenceToBuilding();
        building.SetPosition(new Vector3(data.position.x, 0, data.position.y));
        building.index = data.index;

        buildingCount[data.index]++;
        building.count = buildingCount[data.index];
        buildingGradeCount[(int)building.type, building.grade]++;

        building.happinessRate = data.happiness;
        building.happinessDifference = data.happinessDifference;

        for(int i = 0;i < data.curEvents.Count; i++)
        {
            building.LoadEvent(data.curEvents[i].Load());
        }

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
                if (GameManager.instance.curMapType == MapType.Utopia)
                    buildings.Add(building as CultureBuilding);
                else if (GameManager.instance.curMapType == MapType.Totopia)
                {
                    buildings.Add(building as EnterBuilding);
                    gameBuildings.Add(building as EnterBuilding);
                }
                break;
            case BuildingType.Service:
                buildings.Add(building as ServiceBuilding);
                break;
        }
    }

    public void RemoveBuilding(GameObject building, float second = 0.0f)
    {
        for (int i = 0;i < buildings.Count; i++)
        {
            if (building.GetComponent<Building>().position == buildings[i].position)
            {
                RoutineManager.instance.SetCityHappiness(buildings[i].happinessRate * -1, -1);
                buildings.RemoveAt(i);

                if (GameManager.instance.curMapType == MapType.Totopia && building.GetComponent<Building>().type == BuildingType.Culture)
                    gameBuildings.Remove(building.GetComponent<EnterBuilding>());
                break;
            }
        }

        Building buildingComp = building.GetComponent<Building>();
        buildingGradeCount[(int)buildingComp.type, buildingComp.grade]--;
        buildingRemoveCount++;

        CityLevelManager.instance.curFrames.Enqueue(new FrameInfo() { index = 0, position = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z), rotation = building.transform.rotation, isInsert = false });

        if (buildings.Count < 1)
            AkSoundEngine.PostEvent("Play_construction", gameObject);

        Vector2Int pos = buildingComp.position;
        Grid.instance.tiles[pos.x, pos.y].smokeFX.Play(true);
        EventManager.instance.SetEventBuildings(buildingComp, false);
        AkSoundEngine.PostEvent("Play_Demolition_001_v1", gameObject);

        buildingComp.DestroyBuilding();
        PlayDestroy(building, second);
    }

    private void PlayDestroy(GameObject building, float second)
    {
        building.GetComponent<Collider>().enabled = false;

        building.transform.DOMoveY(-2.2f, second).OnComplete(() =>
        {
            Destroy(building);
        });
    }

    public void ChangeViewState(ViewStateType stateType)
    {
        foreach(Building building in buildings)
        {
            building.ChangeViewState(stateType);
        }
    }

    private void EventBuildingsHighlight()
    {
        isHighlight = !isHighlight;

        if (isHighlight)
        {
            foreach (Building building in buildings)
            {
                TemporayUI temp = UIManager.instance.SetEventHighlightPopUp(building.curEvents.ToArray(), building.transform.position);
                eventIconTemps.Add(temp);
            }
        }
        else
        {
            foreach (TemporayUI temp in eventIconTemps)
                temp.offUI = true;

            eventIconTemps.Clear();
        }
    }

    public void EventHighlightUpdate()
    {
        isHighlight = true;
        EventBuildingsHighlight();
        EventBuildingsHighlight();
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
    
    public int[] GetBuildingsHappiness()
    {
        int[] cnt = new int[4];
        int[] happiness = new int[4];

        foreach (Building building in buildings)
        {
            cnt[(int)building.type]++;
            happiness[(int)building.type] += building.happinessRate;
        }

        for(int i = 0;i < cnt.Length; i++)
        {
            if (cnt[i] != 0)
                happiness[i] /= cnt[i];
        }

        return happiness;
    }

    public int[] GetBuildingsResident()
    {
        int[] resident = new int[3];

        foreach (Building building in buildings)
        {
            if (building.values.ContainsKey(ValueType.Resident))
                resident[3 - building.grade] += (int)building.values[ValueType.Resident].cur;
        }

        return resident;
    }

    public void Notify(EventState state)
    {
        if (state == EventState.EventIcon)
        {
            EventBuildingsHighlight();
        }
    }
}
