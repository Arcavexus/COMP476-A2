using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public int clusterID;
    List<Pathnode> nodesList;

    public Cluster(int id)
    {
        clusterID = id;
        nodesList = new List<Pathnode>();
    }

    public void AddNode(Pathnode node)
    {
        nodesList.Add(node);
    }

    public List<Pathnode> getClusterNodes()
    {
        return nodesList;
    }
    public override bool Equals(System.Object cluster)
    {
        if ((cluster == null) || ! this.GetType().Equals(cluster.GetType())) 
        {
             return false;
        }
        else
        {
            Cluster c = (Cluster) cluster;
            return c.clusterID == this.clusterID;
        }
        
    }
}
