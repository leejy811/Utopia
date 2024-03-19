using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ServiceBuilding : Building
{
    [SerializeField] private int costPerDay;

    [SerializeField] private BoundaryValue employmentValue;

    public override int CalculateIncome()
    {
        int tax = costPerDay * (100 - happinessRate);
        return happinessRate < 40 ? ((int)(tax * 1.5f)) : tax;
    }

    public override void UpdateHappiness()
    {
        //employmentValue
        if (employmentValue.cur > employmentValue.max)
        {
            happinessRate += 1;
            //ToDo
            //영향력 범위 늘리기 추가
        }
        else if (employmentValue.cur < employmentValue.min)
            happinessRate -= 1;
    }
}
