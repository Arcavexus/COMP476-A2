using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode : MonoBehaviour
{
	public Vector3 position;
	public float heuristic;
	public List<Pathnode> neighbourNodes;
    public int clusterID;
    public Pathnode previous;

    void Awake()
    {
        position = transform.position;
    }

	public List<Pathnode> GetNeighbours()
	{
		return neighbourNodes;
	}
    public override bool Equals(System.Object node)
    {
        if ((node == null) || ! this.GetType().Equals(node.GetType())) 
        {
             return false;
        }
        else
        {
            Pathnode n = (Pathnode) node;
            return (position == n.position && heuristic == n.heuristic);
        }
        
    }
}
