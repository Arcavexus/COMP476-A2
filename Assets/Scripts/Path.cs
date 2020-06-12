using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    List<Edge> path;
    int index;

    public Path()
    {
        path = new List<Edge>();
        index = 0;
    }

    public Path(List<Edge> path)
    {
        this.path = path;
        index = 0;
    }

    private void Update()
    {
        foreach(Edge edge in path)
        {
            Debug.DrawLine(edge.getFromNode().position, edge.getToNode().position, Color.red);
        }
    }
    

    public Pathnode firstNode()
    {
        return path[0].getToNode();
    }

    public Pathnode nextNode()
    {
        if(hasNextNode()) {
            index++; 
            return path[index].getToNode();
        }
        return null;
    }

    public bool hasNextNode()
    {
        return index + 1 < path.Count;
    }

    public float computeTotalCost()
    {
        float cost = 0f;
        foreach(Edge edge in path)
        {
            cost += edge.cost;
        }

        return cost;
    }

    public List<Edge> getEdges()
    {
        return path;
    }
}
