using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ServiceBuilding : Building
{
    public int costPerDay;

    public BoundaryValue employmentValue;

    private void Awake()
    {
        values[ValueType.Employment] = employmentValue;
    }

    public override int CalculateIncome()
    {
        float tax = costPerDay * (1.0f - (happinessRate / 100.0f));
        return happinessRate < 40 ? ((int)(tax * 1.5f)) : (int)tax;
    }

    public override void UpdateHappiness()
    {
        //employmentValue
        if (values[ValueType.Employment].cur > values[ValueType.Employment].max)
        {
            happinessRate += 1;
            //ToDo
            //영향력 범위 늘리기 추가
        }
        else if (values[ValueType.Employment].cur < values[ValueType.Employment].min)
            happinessRate -= 1;
    }
}
