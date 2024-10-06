using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

[Serializable]
public class Data
{
    public int level;
    public MoneyData moneyData;
    public List<BuildingIncomeData> buildingIncomeData;
    public List<BuildingData> buildingDatas;
    public List<FrameInfo> prevFrames;
    public List<FrameInfo> curFrames;
    public List<EventData> curEventDatas;
    public List<EventData> globalEventDatas;
    public List<MailData> mailDatas;
    public CreditPanelData creditPanelData;
    public LevelPanelData levelPanelData;
}

[Serializable]
public class MoneyData
{
    public int money;
    public int debt;
    public int creditRating;
    public int payFailTime;
    public int paySuccessTime;
    public int totalIncome;
    public bool isPay;
    public ResultType weekResult;

    public void Save()
    {
        money = ShopManager.instance.Money;

        RoutineManager manager = RoutineManager.instance;
        creditRating = manager.creditRating;
        debt = manager.debt;
        payFailTime = manager.payFailTime;
        paySuccessTime = manager.paySuccessTime;
        totalIncome = manager.totalIncome;
        isPay = manager.isPay;
        weekResult = manager.weekResult;
    }

    public void Load()
    {
        ShopManager.instance.Money = money;

        RoutineManager manager = RoutineManager.instance;
        manager.creditRating = creditRating;
        manager.debt = debt;
        manager.payFailTime = payFailTime;
        manager.paySuccessTime = paySuccessTime;
        manager.totalIncome = totalIncome;
        manager.isPay = isPay;
        manager.weekResult = weekResult;
    }
}

[Serializable]
public class EventData
{
    public int index;
    public int curday;

    public EventData(Event e)
    {
        Save(e);
    } 

    public void Save(Event e)
    {
        index = e.eventIndex;
        curday = e.curDay;
    }

    public Event Load()
    {
        Event e = EventManager.instance.events[index];
        e.curDay = curday;

        return e;
    }
}

[Serializable]
public class BuildingData
{
    public int index;
    public Vector2Int position;
    public Quaternion rotation;
    public int happiness;
    public int happinessDifference;
    public float parameter;
    public List<EventData> curEvents = new List<EventData>();

    public BuildingData(Building building)
    {
        Save(building);
    }

    public void Save(Building building)
    {
        index = building.index;
        position = building.position;
        rotation = building.transform.rotation;

        happiness = building.happinessRate;
        happinessDifference = building.happinessDifference;

        if (building.type == BuildingType.Residential)
            parameter = building.values[ValueType.Resident].cur;
        else
            parameter = building.values[ValueType.utility].cur;

        curEvents.Clear();
        for (int i = 0;i < building.curEvents.Count; i++)
        {
            curEvents.Add(new EventData(building.curEvents[i]));
        }
    }

    public void Load()
    {
        BuildingSpawner.instance.LoadBuilding(this);
    }
}

[Serializable]
public class BuildingIncomeData
{
    public int income;
    public int bonusIncome;
    public int bonusCost;

    public BuildingIncomeData(BuildingType type)
    {
        Save(type);
    }

    public void Save(BuildingType type)
    {
        switch(type)
        {
            case BuildingType.Residential:
                income = ResidentialBuilding.income;
                bonusIncome = ResidentialBuilding.bonusIncome;
                bonusCost = ResidentialBuilding.bonusCost;
                break;
            case BuildingType.Commercial:
                income = CommercialBuilding.income;
                bonusIncome = CommercialBuilding.bonusIncome;
                bonusCost = CommercialBuilding.bonusCost;
                break;
            case BuildingType.Culture:
                income = CultureBuilding.income;
                bonusIncome = CultureBuilding.bonusIncome;
                bonusCost = CultureBuilding.bonusCost;
                break;
            case BuildingType.Service:
                income = ServiceBuilding.income;
                bonusIncome = ServiceBuilding.bonusIncome;
                bonusCost = ServiceBuilding.bonusCost;
                break;
        }
    }

    public void Load(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Residential:
                ResidentialBuilding.income = income;
                ResidentialBuilding.bonusIncome = bonusIncome;
                ResidentialBuilding.bonusCost = bonusCost;
                break;
            case BuildingType.Commercial:
                CommercialBuilding.income = income;
                CommercialBuilding.bonusIncome = bonusIncome;
                CommercialBuilding.bonusCost = bonusCost;
                break;
            case BuildingType.Culture:
                CultureBuilding.income = income;
                CultureBuilding.bonusIncome = bonusIncome;
                CultureBuilding.bonusCost = bonusCost;
                break;
            case BuildingType.Service:
                ServiceBuilding.income = income;
                ServiceBuilding.bonusIncome = bonusIncome;
                ServiceBuilding.bonusCost = bonusCost;
                break;
        }
    }
}

[Serializable]
public class MailData
{
    public MailType mailType;
    public CreditPanelData creditPanelData;
    public LevelPanelData levelPanelData;
    public DayData mailDay;
    public bool isClick;

    public MailData()
    {
        mailDay = new DayData();
    }

    public void Save(MailType mailType, CreditPanelData panelData, bool isClick)
    {
        this.mailType = mailType;
        creditPanelData = panelData;
        mailDay.Save(panelData.day);
        this.isClick = isClick;
    }

    public void Save(MailType mailType, LevelPanelData panelData, bool isClick)
    {
        this.mailType = mailType;
        levelPanelData = panelData;
        mailDay.Save(panelData.day);
        this.isClick = isClick;
    }
}

[Serializable]
public class MapData
{
    public MapType mapType;
    public float playTime;
    public DayData day;
    public int destroyCount;
    public int happiness;
    public int[] buildingTypeCount = new int[System.Enum.GetValues(typeof(BuildingType)).Length];

    public MapData()
    {
        day = new DayData();
    }

    public void Save(MapType mapType)
    {
        this.mapType = mapType;
        playTime = RoutineManager.instance.playTime;
        day.Save(RoutineManager.instance.day);
        destroyCount = BuildingSpawner.instance.buildingRemoveCount;
        happiness = (int)RoutineManager.instance.cityHappiness;

        int[,] count = BuildingSpawner.instance.buildingGradeCount;
        for (int i = 0;i < buildingTypeCount.Length; i++)
        {
            buildingTypeCount[i] = count[i, 0] + count[i, 1] + count[i, 2];
        }
    }

    public void Load()
    {
        RoutineManager.instance.playTime = playTime;
        RoutineManager.instance.day = day.Load();
        BuildingSpawner.instance.buildingRemoveCount = destroyCount;
    }
}

[Serializable]
public class DayData
{
    public int year;
    public int month;
    public int day;

    public void Save(DateTime day)
    {
        year = day.Year;
        month = day.Month;
        this.day = day.Day;
    }

    public DateTime Load()
    {
        DateTime data = new DateTime(year, month, day);
        return data;
    }
}

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public Data data;
    public MapData[] mapData;
    public MapData[] clearData;

    private string path;
    private string fileName;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        path = Application.persistentDataPath;
        fileName = "/SaveData";
        InitMapData();
    }
    
    private void InitMapData()
    {
        mapData = new MapData[Enum.GetValues(typeof(MapType)).Length];
        clearData = new MapData[Enum.GetValues(typeof(MapType)).Length];

        for(int i = 0;i < Enum.GetValues(typeof(MapType)).Length; i++)
        {
            clearData[i] = LoadMapData((MapType)i, "/ClearData_");
            mapData[i] = LoadMapData((MapType)i, "/MapData_");
        }
    }

    public void SaveMapData(MapData data, MapType mapType, string fileName)
    {
        data.Save(mapType);

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + fileName + mapType.ToString(), jsonData);
    }

    public MapData LoadMapData(MapType mapType, string fileName)
    {
        if (!File.Exists(path + fileName + mapType.ToString())) return new MapData();

        string jsonData = File.ReadAllText(path + fileName + mapType.ToString());
        return JsonUtility.FromJson<MapData>(jsonData);
    }

    public void Save()
    {
        data.level = CityLevelManager.instance.levelIdx;
        data.moneyData.Save();
        data.prevFrames = CityLevelManager.instance.prevFrames.ToList();
        data.curFrames = CityLevelManager.instance.curFrames.ToList();

        data.buildingIncomeData.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(BuildingType)).Length; i++)
            data.buildingIncomeData.Add(new BuildingIncomeData((BuildingType)i));

        data.buildingDatas.Clear();
        for (int i = 0; i < BuildingSpawner.instance.buildings.Count; i++)
            data.buildingDatas.Add(new BuildingData(BuildingSpawner.instance.buildings[i]));

        data.curEventDatas.Clear();
        for (int i = 0; i < EventManager.instance.curEvents.Count; i++)
            data.curEventDatas.Add(new EventData(EventManager.instance.curEvents[i]));

        data.globalEventDatas.Clear();
        for (int i = 0; i < EventManager.instance.globalEvents.Count; i++)
            data.globalEventDatas.Add(new EventData(EventManager.instance.globalEvents[i]));

        data.mailDatas.Clear();
        for (int i = 0; i < UIManager.instance.phone.mailDatas.Count; i++)
            data.mailDatas.Add(UIManager.instance.phone.mailDatas[i]);

        data.creditPanelData = UIManager.instance.phone.panels[(int)PhoneState.Credit].GetComponent<CreditPanelUI>().data as CreditPanelData;
        data.levelPanelData = UIManager.instance.phone.panels[(int)PhoneState.Level].GetComponent<LevelPanelUI>().data as LevelPanelData;

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + fileName, jsonData);

        MapType type = GameManager.instance.curMapType;
        SaveMapData(mapData[(int)type], type, "/MapData_");
    }

    public void Load()
    {
        string jsonData = File.ReadAllText(path + fileName);
        data = JsonUtility.FromJson<Data>(jsonData);

        data.moneyData.Load();
        CityLevelManager.instance.LoadLevel(data.level);
        CityLevelManager.instance.LoadFrame(data.prevFrames.ToArray(), data.curFrames.ToArray());

        for (int i = 0; i < Enum.GetValues(typeof(BuildingType)).Length; i++)
            data.buildingIncomeData[i].Load((BuildingType)i);

        for (int i = 0; i < data.buildingDatas.Count; i++)
            data.buildingDatas[i].Load();

        for (int i = 0; i < data.curEventDatas.Count; i++)
            EventManager.instance.curEvents.Add(data.curEventDatas[i].Load());

        for (int i = 0; i < data.globalEventDatas.Count; i++)
            EventManager.instance.globalEvents.Add(data.globalEventDatas[i].Load());

        for (int i = 0; i < data.mailDatas.Count; i++)
            UIManager.instance.phone.LoadMail(data.mailDatas[i]);

        UIManager.instance.phone.SetPanelData(PhoneState.Credit, data.creditPanelData);
        UIManager.instance.phone.SetPanelData(PhoneState.Level, data.levelPanelData);

        mapData[(int)GameManager.instance.curMapType].Load();

        UIManager.instance.InitInfoUI();
    }
}
