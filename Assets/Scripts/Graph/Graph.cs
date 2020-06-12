using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static List<Pathnode> nodesList = new List<Pathnode>();
    public static List<Edge> edgesList = new List<Edge>();
    static Path path;
    static List<Edge> visitedPath = new List<Edge>();
    public static Cluster[] clusterArray = new Cluster[6];
    static float[,] lookupTable = new float[6, 6];
    
    void Awake()
    {
        InitializeClusterMapAndLookupTable();
        foreach(Cluster c in clusterArray)
        {
            Debug.Log(c.clusterID);
        }

        // populate nodes
        GameObject nodesParent = GameObject.Find("Pathnodes");
        foreach(Transform child in nodesParent.transform)
        {
            Pathnode node = child.GetComponent<Pathnode>();
            nodesList.Add(node);
            clusterArray[node.clusterID].AddNode(node);
        }

        // populate edges
        foreach(Pathnode node in nodesList)
        {
            foreach(Pathnode neighbour in node.GetNeighbours())
            {
                edgesList.Add(new Edge(node, neighbour, (node.position - neighbour.position).magnitude));
            }
        }
        FindShortestPath(new Vector3(-4.6f, 0f, -4.6f), new Vector3(4.4f, 0f, 4.4f));
        //CreateClusterLookupTable();
        Debug.Log("done");
    }

    public void OnDrawGizmos()
	{
        foreach(Edge edge in edgesList)
        {
            Gizmos.DrawLine(edge.getFromNode().position, edge.getToNode().position);
        }
        /*
        foreach(Edge edge in visitedPath)
        {
            Debug.DrawLine(edge.getFromNode().position, edge.getToNode().position, Color.red);
        }
        foreach(Edge edge in path.getEdges())
        {
            Debug.DrawLine(edge.getFromNode().position, edge.getToNode().position, Color.yellow);
        }*/
        
	}

    Edge getEdge(Pathnode nodeFrom, Pathnode nodeTo)
    {
        foreach(Edge edge in edgesList)
        {
            if(edge.getFromNode() == nodeFrom && edge.getToNode() == nodeTo)
            {
                Debug.Log("found a match");
                return edge;
            }
        }
        return null;
    }

    // backtracks the path from the endNode and connects it to its previous nodes
    static List<Edge> connectPath(Pathnode startNode, Pathnode endNode)
    {
        List<Edge> connectedPath = new List<Edge>();
        Pathnode prevNode = endNode;
        do
        {
            float dist = (prevNode.position - prevNode.previous.position).magnitude;
            Edge edge = new Edge(prevNode.previous, prevNode, dist);
            connectedPath.Insert(0, edge);
            prevNode = prevNode.previous;
        } while(prevNode != startNode);
        
        return connectedPath;
    }

    /**
     * Connects a path between the closest start and end node to two points
     */ 
    public static Path FindShortestPath (Vector3 start, Vector3 end)
    {     
        
        Pathnode nodeClosestToStart = GetNodeClosestTo(start);
        Pathnode nodeClosestToEnd = GetNodeClosestTo(end);

        GameObject startObject = new GameObject("startObject");
        startObject.transform.position = start;
        startObject.AddComponent<Pathnode>();

        GameObject endObject = new GameObject("endObject");
        endObject.transform.position = end;
        endObject.AddComponent<Pathnode>();

        // create a node at start and end
        Pathnode startNode = startObject.GetComponent<Pathnode>();
        Pathnode endNode = endObject.GetComponent<Pathnode>();

        if(start == end)
        {
            return null;
        }

        if(startNode.Equals(nodeClosestToStart))
        {
            startNode = nodeClosestToStart;
        }
        else
        {
            startNode.neighbourNodes = new List<Pathnode>();
            startNode.neighbourNodes.Add(nodeClosestToStart);
            nodeClosestToStart.neighbourNodes.Add(startObject.GetComponent<Pathnode>());
        }

        if(endNode.Equals(nodeClosestToEnd))
        {
            endNode = nodeClosestToEnd;
        }
        else
        {
            endNode.neighbourNodes = new List<Pathnode>();
            endNode.neighbourNodes.Add(nodeClosestToEnd);
            nodeClosestToEnd.neighbourNodes.Add(endObject.GetComponent<Pathnode>());
        }

        Pathnode currentNode = startNode;
		
        List<Pathnode> openList = new List<Pathnode>();
        List<Pathnode> closedList = new List<Pathnode>();
        closedList.Add(currentNode);
        
		int iter = 0;
		do {
			iter ++;
            //Debug.Log("START OF ITERATION " + iter);
            //Debug.Log("currentNode is " + currentNode.name);

			float lowestCost = float.MaxValue;
            float cost = 0;
            Pathnode closestNode = null;
            Pathnode toNode = null;
            Pathnode fromNode = null;

            //Debug.Log("looking for " + currentNode.name + "'s neighbours");

            /*Debug.Log("Neighbours = " + string.Join("",
             new List<Pathnode>(currentNode.neighbourNodes)
             .ConvertAll(i => i.ToString())
             .ToArray()));*/

            // add to open list
			foreach(Pathnode neighbour in currentNode.neighbourNodes)
            {
                if (!openList.Contains(neighbour) && !closedList.Contains(neighbour))
                {
                    neighbour.previous = currentNode;
                    openList.Add(neighbour);
                    //Debug.Log("added " + neighbour.name + " to open list");
                }
            }

            /*Debug.Log("OpenList = " + string.Join("",
             new List<Pathnode>(openList)
             .ConvertAll(i => i.ToString())
             .ToArray()));
            Debug.Log("ClosedList = " + string.Join("",
             new List<Pathnode>(closedList)
             .ConvertAll(i => i.ToString())
             .ToArray()));*/

            foreach (Pathnode openNode in openList) {
                if(openNode.Equals(currentNode)) // skip current node if already in open list
                {
                    continue;
                }
                float edgeCost = float.MaxValue;
                if(openNode.clusterID != openNode.previous.clusterID)
                {
                    // lookup table for cluster values
                    edgeCost = lookupTable[openNode.clusterID, openNode.previous.clusterID];
                }
                else
                { 
                    // otherwise compute euclidean distance as the heuristic
                    edgeCost = (openNode.position - openNode.previous.position).magnitude;
                }
                
                cost = edgeCost + (endNode.position - openNode.position).magnitude;
                //Debug.Log("edge cost from " + openNode.previous.name + " to " + openNode.name + " is " + cost);

                // Find lowest cost path
			    if (cost < lowestCost && !closedList.Contains(openNode)) {
                    
				    lowestCost = cost;
				    closestNode = openNode;
                    
                    //Debug.Log("cost is lower than before, new closest node is now " + closestNode.name);

                    toNode = openNode;
                    fromNode = openNode.previous;
			    }
            }
            //Debug.Log("adding " + currentNode.name + " to closedlist");
            //Debug.Log("closest node is " + closestNode.name);
            closedList.Add(currentNode);
            //Debug.Log("removing " + currentNode.name + " from the open list");
            openList.Remove(currentNode);
			//pathNodes.Add(currentNode);
			//path.Add(currentNode); to replace
            visitedPath.Add(new Edge(fromNode, toNode, lowestCost));
            //Debug.Log("next node will be " + closestNode.name);
			currentNode = closestNode;

            //Debug.Log("END OF ITERATION " + iter);
		} while(currentNode != endNode && iter < 1000);
        
        // finally, connect the path
        path = new Path(connectPath(startNode, endNode));

        nodeClosestToStart.neighbourNodes.Remove(startNode);
        nodeClosestToEnd.neighbourNodes.Remove(endNode);

        Destroy(startObject);
        Destroy(endObject);

        return path;
    }

    public static Pathnode GetNodeClosestTo(Vector3 pos)
	{
		if (nodesList == null || nodesList.Count <= 0)
			return null;
		else {
			float shortestDistance = float.MaxValue;
            Pathnode closestNode = null;
			for (int i = 0; i < nodesList.Count; ++i) {
				Pathnode node = nodesList [i];
                float dist = Vector3.Distance(pos, node.position);
                if (dist < shortestDistance)
                {
					shortestDistance = dist;
					closestNode = node;
				}
			}
			return closestNode;
		}
	}

    // computes the shortest path from any node from a cluster X to any node in a cluster Y and stores it in a lookup table
    public void CreateClusterLookupTable()
    {      
        for(int i = 0; i < clusterArray.Length; i++)
        {
            for(int j = i; j < clusterArray.Length; j++)
            {
                if(clusterArray[i].Equals(clusterArray[j]))
                {
                    continue;
                }
                float lowestCost = float.MaxValue;
                foreach(Pathnode nodeFrom in clusterArray[i].getClusterNodes())
                {
                    foreach(Pathnode nodeTo in clusterArray[j].getClusterNodes())
                    {
                        path = FindShortestPath(nodeFrom.position, nodeTo.position);
                        if(path != null)
                        {
                            float cost = path.computeTotalCost();
                            if(cost < lowestCost)
                            {
                                lowestCost = cost;
                            }
                        }
                    }
                }
                lookupTable[i, j] = lowestCost;
                lookupTable[j, i] = lowestCost;
            }
        }
    }

    public void InitializeClusterMapAndLookupTable()
    {
        for(int i = 0; i < clusterArray.Length; i++)
        {
            clusterArray[i] = new Cluster(i);
        }

        // initialize lookup table with values from pre-computed paths in CreateClusterLookupTable()
        // this is the result from running the above function
        lookupTable = new float [6, 6] {
            {    0f,    1.29f,     9.81f,     7.27f,    5.89f,       4.9f   },
            { 1.29f,       0f,     6.69f,     9.39f,   10.45f,      1.17f   },
            { 9.81f,    6.69f,        0f,     4.83f,    5.33f,      1.07f   },
            { 7.27f,    9.39f,     4.83f,        0f,    3.36f,      1.15f   },
            { 5.89f,   10.45f,     5.33f,     3.36f,       0f,      0.99f   },
            {  4.9f,    1.17f,     1.07f,     1.15f,    0.99f,         0f   }
            };


           
    }
}
