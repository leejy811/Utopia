using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PanelData
{
    public DayData day = new DayData();
}

[Serializable]
public class CreditPanelData : PanelData
{
    public int money;
    public int score;
    public int debt;
    public ResultType result;

    public CreditPanelData(int money, int score, int debt, ResultType result, DateTime day)
    {
        this.money = money;
        this.score = score;
        this.debt = debt;
        this.result = result;
        this.day.Save(day);
    }
}

[Serializable]
public class LevelPanelData : PanelData
{
    public int level;

    public LevelPanelData(int level, DateTime day) 
    { 
        this.level = level;
        this.day.Save(day);
    }
}