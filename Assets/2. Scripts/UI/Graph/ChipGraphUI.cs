using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SquareGraphInfo
{
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float width;
    public Color color;

    public virtual void DrawSquare(VertexHelper vh)
    {
        UIVertex[] verts = new UIVertex[4];

        verts[0].position = new Vector3(startPoint.x - width / 2, startPoint.y, 0);
        verts[1].position = new Vector3(endPoint.x - width / 2, endPoint.y, 0);
        verts[2].position = new Vector3(endPoint.x + width / 2, endPoint.y, 0);
        verts[3].position = new Vector3(startPoint.x + width / 2, startPoint.y, 0);

        for (int i = 0; i < 4; i++)
        {
            verts[i].color = color;
        }

        vh.AddUIVertexQuad(verts);
    }
}

public class ChipGraphUI : MaskableGraphic, IScrollHandler, IDragHandler
{
    public float padding;
    public int dragThreshold;
    public Color increaseColor;
    public Color decreaseColor;

    private Dictionary<DateTime, int> chipCostDatas;

    private DateTime minDay;
    private DateTime maxDay;
    private int mincost;
    private int maxcost;
    private int dragCount;

    protected override void OnEnable()
    {
        InitGraph();

        base.OnEnable();
    }

    private void InitGraph()
    {
        maxDay = RoutineManager.instance.day;
        minDay = RoutineManager.instance.day.Subtract(new TimeSpan(6, 0, 0, 0));

        if (minDay < new DateTime(2024, 1, 1))
            minDay = new DateTime(2024, 1, 1);

        Debug.Log(maxDay.ToString() + "/" + minDay.ToString() + "/" + (maxDay - minDay).Days);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        chipCostDatas = ChipManager.instance.chipCostDatas;

        CalcMinMaxCost();

        for (int i = 0; i <= (maxDay - minDay).Days; i++)
        {
            SquareGraphInfo graph = DayToGraph(minDay.AddDays(i));
            graph.DrawSquare(vh);

            if ((maxDay - minDay).Days == i)
            {
                Debug.Log(graph.startPoint.x);
            }
        }
    }

    private SquareGraphInfo DayToGraph(DateTime day)
    {
        SquareGraphInfo stick = new SquareGraphInfo();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float costHeight = maxcost - mincost;
        int dayWidth = Mathf.Max((maxDay - minDay).Days + 1, 7);

        int prevCost = chipCostDatas[day.Subtract(new TimeSpan(1, 0, 0, 0))];
        int curCost = chipCostDatas[day];

        stick.width = width / (dayWidth + padding * dayWidth);
        stick.color = prevCost > curCost ? decreaseColor : increaseColor;

        float xPos = stick.width * (1 + padding) * (0.5f + (day - minDay).Days);
        float startYPos = (prevCost - mincost) / costHeight * height;
        float endYPos = (curCost - mincost) / costHeight * height;

        stick.startPoint = new Vector2(xPos, startYPos);
        stick.endPoint = new Vector2(xPos, endYPos);

        return stick;
    }

    private void CalcMinMaxCost()
    {
        List<int> cost = new List<int>();

        for (int i = 0;i <= (maxDay - minDay).Days; i++)
        {
            cost.Add(chipCostDatas[minDay.AddDays(i)]);
        }
        cost.Add(chipCostDatas[minDay.Subtract(new TimeSpan(1, 0, 0, 0))]);

        mincost = cost.Min();
        maxcost = cost.Max();
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.scrollDelta.y > 0)
        {
            if ((maxDay - minDay).Days <= 7) return;

            minDay = minDay.AddDays(1);
            maxDay = maxDay.Subtract(new TimeSpan(1, 0, 0, 0));
        }
        else
        {
            maxDay = maxDay.AddDays(1);
            minDay = minDay.Subtract(new TimeSpan(1, 0, 0, 0));

            if (minDay < new DateTime(2024, 1, 1))
                minDay = new DateTime(2024, 1, 1);

            if (maxDay > RoutineManager.instance.day)
                maxDay = RoutineManager.instance.day;
        }

        SetAllDirty();
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragCount++;

        if (dragCount > dragThreshold)
        {
            if (eventData.delta.x > 0)
            {
                dragCount = 0;
                if (minDay == new DateTime(2024, 1, 1)) return;

                minDay = minDay.Subtract(new TimeSpan(1, 0, 0, 0));
                maxDay = maxDay.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            else
            {
                dragCount = 0;
                if (maxDay == RoutineManager.instance.day) return;

                minDay = minDay.AddDays(1);
                maxDay = maxDay.AddDays(1);
            }
        }

        SetAllDirty();
    }
}