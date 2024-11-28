using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphHighlight : Graphic
{
    public float ratio;
    public SquareGraphInfo graph;

    private float addWidth;

    public void ChangeGraph(SquareGraphInfo graph)
    {
        if (graph == null)
        {
            addWidth = 0.0f;
            this.graph = null;
            return;
        }

        this.graph = new SquareGraphInfo(graph);
        addWidth = graph.width * ratio;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if(graph != null)
        {
            graph.width += addWidth;

            int sign = graph.startPoint.y < graph.endPoint.y ? 1 : -1;
            graph.startPoint = new Vector2(graph.startPoint.x, graph.startPoint.y - addWidth / 2 * sign);
            graph.endPoint = new Vector2(graph.endPoint.x, graph.endPoint.y + addWidth / 2 * sign);
            graph.color = Color.white;
            graph.DrawSquare(vh);
        }
    }
}