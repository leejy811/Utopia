using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public enum RoadType { Straight = 0, Corner, Threeway, Crossway }

public class RoadPlacement : MonoBehaviour
{
    public GameObject[] roads;

    public void PlaceRoad(Transform tileTrans)
    {
        bool[] isRoads = Grid.instance.GetRoadInformation(tileTrans);
        int cnt = 0;

        for (int i = 0;i < roads.Length;i++)
        {
            if (isRoads[i]) cnt++;
            roads[i].gameObject.SetActive(false);
        }
        if (cnt == 0) cnt++;

        RoadType type;

        if (cnt == 2)
        {
            if ((isRoads[0] && isRoads[2]) || (isRoads[1] && isRoads[3]))
                type = RoadType.Straight;
            else
                type = RoadType.Corner;
        }
        else
            type = (RoadType)(cnt - 1);

        RotateRoad(type, isRoads);
    }

    private void RotateRoad(RoadType type, bool[] isRoad)
    {
        float angle = 0f;
        roads[(int)type].gameObject.SetActive(true);

        if (type == RoadType.Straight && (isRoad[0] || isRoad[2]))
            angle = 90f;
        else if (type == RoadType.Corner)
        {
            bool flag = false;
            int i;
            for (i = 0;i <= isRoad.Length;i++)
            {
                int idx = i % isRoad.Length;
                if (flag && isRoad[idx]) break;

                flag = isRoad[idx];
            }
            angle = (i - 1) * 90f;
        }
        else if (type == RoadType.Threeway)
        {
            int i;
            for (i = 0; i < isRoad.Length; i++)
            {
                if (!isRoad[i]) break;
            }
            angle = (i + 2) * 90f;
        }

        roads[(int)type].transform.localEulerAngles = Vector3.up * angle;
    }
}
