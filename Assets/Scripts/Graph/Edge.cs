using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Pathnode fromNode;
    public Pathnode toNode;
    public float cost;

    public Edge()
    {
        fromNode = null;
        toNode = null;
        cost = 0;
    }

    public Edge(Pathnode fromNode, Pathnode toNode, float cost)
    {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.cost = cost;
    }

    public Pathnode getFromNode()
    {
        return fromNode;
    }

    public Pathnode getToNode()
    {
        return toNode;
    }

    public override bool Equals(System.Object edge)
    {
        if ((edge == null) || ! this.GetType().Equals(edge.GetType())) 
        {
             return false;
        }
        else
        {
            Edge e = (Edge) edge;
            return (fromNode.Equals(e.getFromNode()) && toNode.Equals(e.getToNode()));
        }
        
    }
}
