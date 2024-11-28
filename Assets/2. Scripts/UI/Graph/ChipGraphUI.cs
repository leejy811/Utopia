using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
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

    public SquareGraphInfo(Vector2 startPoint, Vector2 endPoint, float width, Color color)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.width = width;
        this.color = color;
    }

    public SquareGraphInfo(SquareGraphInfo graph)
    {
        startPoint = graph.startPoint;
        endPoint = graph.endPoint;
        width = graph.width;
        color = graph.color;
    }
}

public class ChipGraphUI : Graphic, IScrollHandler, IDragHandler
{
    public float padding;
    public int dragThreshold;
    public Color increaseColor;
    public Color decreaseColor;
    public Camera uiCamera;
    public GraphHighlight highlight;

    private Dictionary<DateTime, int> chipCostDatas;

    private DateTime minDay;
    private DateTime maxDay;
    private int mincost;
    private int maxcost;
    private int dragCount;
    private List<SquareGraphInfo> graphs = new List<SquareGraphInfo>();
    private int pickIdx;
    private TradeChipUI chipUI;

    protected override void Start()
    {
        base.Start();

        chipUI = gameObject.GetComponentInParent<TradeChipUI>();
    }

    protected override void OnEnable()
    {
        InitGraph();

        base.OnEnable();
    }

    private void Update()
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, uiCamera, out localPos);

        if (0.0f > localPos.y || rectTransform.rect.height < localPos.y)
        {
            pickIdx = -1;
            SetHightlight(null);
            return;
        }

        bool isOnGraph = false;
        int prevIdx = pickIdx;
        for (int i = 0;i < graphs.Count; i++)
        {
            if (graphs[i].startPoint.x - graphs[i].width / 2 < localPos.x
                && graphs[i].startPoint.x + graphs[i].width / 2 > localPos.x)
            {
                pickIdx = i;
                isOnGraph = true;
                break;
            }
        }

        if (!isOnGraph)
        {
            pickIdx = -1;
            SetHightlight(null);
            return;
        }
        else if (prevIdx == pickIdx) return;

        SetHightlight(graphs[pickIdx]);
    }

    private void SetHightlight(SquareGraphInfo graph)
    {
        highlight.ChangeGraph(graph);
        highlight.SetAllDirty();
        chipUI.SetChartInfo(minDay.AddDays(pickIdx), pickIdx != -1);
    }

    private void InitGraph()
    {
        maxDay = RoutineManager.instance.day;
        minDay = RoutineManager.instance.day.Subtract(new TimeSpan(6, 0, 0, 0));

        if (minDay < new DateTime(2024, 1, 1))
            minDay = new DateTime(2024, 1, 1);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        chipCostDatas = ChipManager.instance.chipCostDatas;

        CalcMinMaxCost();

        graphs.Clear();
        for (int i = 0; i <= (maxDay - minDay).Days; i++)
        {
            graphs.Add(DayToGraph(minDay.AddDays(i)));
            graphs[i].DrawSquare(vh);
        }
    }

    private SquareGraphInfo DayToGraph(DateTime day)
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float costHeight = maxcost - mincost;
        int dayWidth = Mathf.Max((maxDay - minDay).Days + 1, 7);

        int prevCost = chipCostDatas[day.Subtract(new TimeSpan(1, 0, 0, 0))];
        int curCost = chipCostDatas[day];

        float gWidth = width / (dayWidth + padding * dayWidth);
        Color color = prevCost > curCost ? decreaseColor : increaseColor;

        float xPos = gWidth * (1 + padding) * (0.5f + (day - minDay).Days);
        float startYPos = (prevCost - mincost) / costHeight * height;
        float endYPos = (curCost - mincost) / costHeight * height;

        Vector2 startPoint = new Vector2(xPos, startYPos);
        Vector2 endPoint = new Vector2(xPos, endYPos);

        SquareGraphInfo stick = new SquareGraphInfo(startPoint, endPoint, gWidth, color);

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