using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBuilding : UtilityBuilding
{
    public static int income;
    public static int bonusIncome;
    public static int bonusCost;

    public int betChip;
    public int betTimes;
    public BoundaryValue RandomChip;

    public override void SetParameter(int sign)
    {
        betChip = (int)GetRandomParameter(sign);
    }

    public override float GetRandomParameter(int sign)
    {
        if (sign == -1)
            return RandomChip.min;
        else if (sign == 0)
            return RandomChip.cur;
        else
            return RandomChip.max;
    }

    public override void ApplyInfluence(int value, BuildingType type, bool isInit = false)
    {
        base.ApplyInfluence(value, type, isInit);

        if (type != this.type) return;

        BoundaryValue cast = values[ValueType.utility];
        cast.cur += value;
        values[ValueType.utility] = cast;

        if (!isInit)
        {
            ExpectHappiness(0);
        }
    }
}
