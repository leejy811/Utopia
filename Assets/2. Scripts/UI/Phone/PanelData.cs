using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelData
{

}

public class CreditPanelData : PanelData
{
    public int money { get; private set; }
    public int score { get; private set; }
    public int debt { get; private set; }
    public ResultType result { get; private set; }
    public DateTime day { get; private set; }

    public CreditPanelData(int money, int score, int debt, ResultType result, DateTime day)
    {
        this.money = money;
        this.score = score;
        this.debt = debt;
        this.result = result;
        this.day = day;
    }
}

public class LevelPanelData : PanelData
{
    public int level { get; private set; }

    public LevelPanelData(int level) { this.level = level; }
}