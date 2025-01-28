using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using System.Security.Cryptography;
using UnityEngine.XR;
using Unity.Mathematics;
using UnityEngine.Rendering;

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
    public float extraParameter;
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
        else if (GameManager.instance.curMapType == MapType.Totopia && building.type == BuildingType.Culture)
        {
            parameter = building.values[ValueType.betChip].cur;
            extraParameter = (building as EnterBuilding).betTimes;
        }
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
        mailDay = panelData.day;
        this.isClick = isClick;
    }

    public void Save(MailType mailType, LevelPanelData panelData, bool isClick)
    {
        this.mailType = mailType;
        levelPanelData = panelData;
        mailDay = panelData.day;
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

[Serializable]
public class SoundData
{
    public float[] volumes;

    public void Save(float[] volumes)
    {
        this.volumes = volumes;
    }

    public float[] Load()
    {
        return volumes;
    }
}

[Serializable]
public class ChipData
{
    public int[] costs;
    public float changeRatio;
    public int curChip;

    public void Save(Dictionary<DateTime, int> costs, float ratio, int chip)
    {
        DateTime startDay = new DateTime(2023, 12, 31);
        DateTime endDay = RoutineManager.instance.day;
        this.costs = new int[(endDay - startDay).Days + 1];

        for (int i = 0;i <= (endDay - startDay).Days; i++)
        {
            this.costs[i] = costs[startDay.AddDays(i)];
        }

        this.changeRatio = ratio;
        this.curChip = chip;
    }

    public Dictionary<DateTime, int> Load()
    {
        Dictionary<DateTime, int> costs = new Dictionary<DateTime, int>();
        DateTime startDay = new DateTime(2023, 12, 31);
        DateTime endDay = RoutineManager.instance.day;
        Debug.Log(endDay);
        for (int i = 0; i <= (endDay - startDay).Days; i++)
        {
            costs[startDay.AddDays(i)] = this.costs[i];
        }
        return costs;
    }
}

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public Data data;
    public MapData[] mapData;
    public MapData[] clearData;
    public SoundData soundData;

    private string path;
    private string fileName;
    private readonly string privateKey = "bcbffk4dm1fgfzzzepl4316sgznvaomitf7lsll";

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
        fileName = "/SaveData_";
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
        SaveFile(jsonData, path + fileName + mapType.ToString());
    }

    public MapData LoadMapData(MapType mapType, string fileName)
    {
        if (!File.Exists(path + fileName + mapType.ToString())) return new MapData();

        string jsonData = LoadFile(path + fileName + mapType.ToString());
        return JsonUtility.FromJson<MapData>(jsonData);
    }

    public void DeleteMapData(MapType mapType, string fileName)
    {
        File.Delete(path + fileName + mapType.ToString());
    }

    public void SaveSoundData(float[] volumes)
    {
        soundData.Save(volumes);

        string jsonData = JsonUtility.ToJson(soundData, true);
        SaveFile(jsonData, path + "/SoundData");
    }

    public float[] LoadSoundData()
    {
        if (!File.Exists(path + "/SoundData"))
        {
            float[] volumes = { 100.0f, 100.0f };
            return volumes;
        }   

        string jsonData = LoadFile(path + "/SoundData");
        soundData = JsonUtility.FromJson<SoundData>(jsonData);
        return soundData.volumes;
    }

    public void SaveChipData(Dictionary<DateTime, int> rawChipData, float ratio, int chip)
    {
        ChipData chipData = new ChipData();
        chipData.Save(rawChipData, ratio, chip);
        string jsonData = JsonUtility.ToJson(chipData, true);
        SaveFile(jsonData, path + "/ChipData");
    }

    public ChipData LoadChipData()
    {
        if (!File.Exists(path + "/ChipData"))
        {
            return null;
        }

        string jsonData = LoadFile(path + "/ChipData");
        ChipData chipData = JsonUtility.FromJson<ChipData>(jsonData);
        return chipData;
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
        SaveFile(jsonData, path + fileName + GameManager.instance.curMapType);

        MapType type = GameManager.instance.curMapType;
        SaveMapData(mapData[(int)type], type, "/MapData_");
    }

    public void Load()
    {
        string jsonData = LoadFile(path + fileName + GameManager.instance.curMapType);
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

        if (GameManager.instance.curMapType == MapType.Totopia)
            ChipManager.instance.InitChipCost();
    }

    private void SaveFile(string jsonData, string path)
    {
        string cryptoString = Encrypt(jsonData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(cryptoString);
        File.WriteAllBytes(path, bytes);
    }

    private string LoadFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        string cryptoString = System.Text.Encoding.UTF8.GetString(bytes);
        string jsonString = Decrypt(cryptoString);
        return jsonString;
    }

    private string Encrypt(string data)
    {

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);
    }

    private string Decrypt(string data)
    {

        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }

    private RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }
}
