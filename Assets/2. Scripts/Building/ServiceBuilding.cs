using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ServiceBuilding : UtilityBuilding
{
    public override int CalculateIncome()
    {
        float tax = costPerDay * (1.0f - (happinessRate / 100.0f)) * -1;
        return happinessRate < 40 ? ((int)(tax * 1.5f)) : happinessRate >= 80 ? ((int)(tax * 0.5f)) : (int)tax;
    }

    public override int CalculateBonus()
    {
        return 0;
    }
}
