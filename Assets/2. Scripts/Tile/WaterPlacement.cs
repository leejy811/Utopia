using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterType { Circle, Quarter, Side, Middle }

public class WaterPlacement : MonoBehaviour
{
    public GameObject[] waters;

    public void PlaceWater(Transform tileTrans)
    {
        bool[] isWaters = Grid.instance.GetWaterInformation(tileTrans);
        int cnt = 0;

        for (int i = 0; i < waters.Length; i++)
        {
            if (isWaters[i]) cnt++;
            waters[i].gameObject.SetActive(false);
        }

        WaterType type = WaterType.Circle;
        float angle = 0f;

        if (isWaters[1] && isWaters[2] && isWaters[4])
        {
            type = WaterType.Quarter;
            angle = 0f;
        }
        else if (isWaters[4] && isWaters[7] && isWaters[6])
        {
            type = WaterType.Quarter;
            angle = 90f;
        }
        else if (isWaters[3] && isWaters[5] && isWaters[6])
        {
            type = WaterType.Quarter;
            angle = 180f;
        }
        else if (isWaters[0] && isWaters[1] && isWaters[3])
        {
            type = WaterType.Quarter;
            angle = 270f;
        }

        if (isWaters[1] && isWaters[2] && isWaters[4] && isWaters[6] && isWaters[7])
        {
            type = WaterType.Side;
            angle = 0f;
        }
        else if (isWaters[3] && isWaters[4] && isWaters[5] && isWaters[6] && isWaters[7])
        {
            type = WaterType.Side;
            angle = 90f;
        }
        else if (isWaters[0] && isWaters[1] && isWaters[3] && isWaters[5] && isWaters[6])
        {
            type = WaterType.Side;
            angle = 180f;
        }
        else if (isWaters[0] && isWaters[1] && isWaters[2] && isWaters[3] && isWaters[4])
        {
            type = WaterType.Side;
            angle = 270f;
        }

        if (isWaters[0] && isWaters[1] && isWaters[2] && isWaters[3] 
            && isWaters[4] && isWaters[5] && isWaters[6] && isWaters[7]) 
            type = WaterType.Middle;

        waters[(int)type].gameObject.SetActive(true);
        waters[(int)type].transform.localEulerAngles = Vector3.up * angle;
    }
}
