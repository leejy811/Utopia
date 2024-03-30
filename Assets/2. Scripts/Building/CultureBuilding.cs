using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CultureBuilding : UtilityBuilding
{
    public override int CalculateBonus()
    {
        int res = values[ValueType.utility].cur > values[ValueType.utility].max ? 1 : 0;
        return res;
    }
}
