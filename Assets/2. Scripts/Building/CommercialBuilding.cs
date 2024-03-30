using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CommercialBuilding : UtilityBuilding
{
    public override int CalculateBonus()
    {
        int res = values[ValueType.utility].cur > values[ValueType.utility].max ? 1 : 0;
        return res;
    }
}
