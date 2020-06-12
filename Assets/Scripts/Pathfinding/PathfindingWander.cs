using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingWander : MonoBehaviour
{
    
    private Vector3 targetLocation;
    public float maxAccel;
    public Vector3 velocity;
    
    private Path pathToFollow;
    private Pathnode lastVisitedNode;
    private Pathnode followedNode;
    private Cluster clusterToSearch;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        clusterToSearch = Graph.clusterArray[Graph.GetNodeClosestTo(transform.position).clusterID];
        followedNode = Graph.GetNodeClosestTo(transform.position);
        NextClusterNode();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 distanceVector = followedNode.position - transform.position;
        float distance = distanceVector.magnitude;

        Vector3 accelVector = (distanceVector / distance) * maxAccel;
        velocity = accelVector  * Time.deltaTime;

        // when we hit a node on our path, go to the next one
        if(distanceVector.magnitude < 0.1f && pathToFollow.hasNextNode())
        {
            lastVisitedNode = followedNode;
            followedNode = pathToFollow.nextNode();
        }
        // when we finally arrive to the last point of our path, generate a new path
        if(distanceVector.magnitude < 0.05f)
        {
            NextClusterNode();
        }
            
        transform.LookAt(followedNode.position);
        transform.position += velocity;
        

    }

    // find next cluster, and visit a random node in that cluster (for searching)
    void NextClusterNode()
    {
        int currentClusterID = Graph.GetNodeClosestTo(transform.position).clusterID;
        clusterToSearch = Graph.clusterArray[(clusterToSearch.clusterID + 1) % Graph.clusterArray.Length];
        Pathnode destination = clusterToSearch.getClusterNodes()[Random.Range(0, clusterToSearch.getClusterNodes().Count)];
        pathToFollow = Graph.FindShortestPath(transform.position, destination.position);
    }

}
