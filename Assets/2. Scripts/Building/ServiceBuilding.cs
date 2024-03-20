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
        int tax = costPerDay * (100 - happinessRate);
        return happinessRate < 40 ? ((int)(tax * 1.5f)) : tax;
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
