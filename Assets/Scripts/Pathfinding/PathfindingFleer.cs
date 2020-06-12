using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingFleer : MonoBehaviour
{

    public enum State { WANDER, FLEE, FROZEN}
    public State currentState;
    public Vector3 targetLocation;
    public List<PathfindingChaser> pursuers;
    public float maxAccel;
    public Vector3 velocity;
    public PathfindingChaser currentPursuer;
    public Pathnode followedNode;

    private Pathnode lastVisitedNode;
    private Path pathToFollow;
    private Bounds bounds;
    private Cluster clusterToSearch;
    
    // Start is called before the first frame update
    void Awake()
    {
        //InvokeRepeating("RandomizeTarget", 0.0f, 3.0f);
        InvokeRepeating("CheckNearbyPursuers", 0.0f, 0.5f);
        InvokeRepeating("Flee2", 0.0f, 0.5f);
        //InvokeRepeating("AvoidBacktracking", 3.0f, 3.0f);

        lastVisitedNode = Graph.GetNodeClosestTo(transform.position);
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;
        followedNode = Graph.GetNodeClosestTo(transform.position);
        clusterToSearch = Graph.clusterArray[Graph.GetNodeClosestTo(transform.position).clusterID];
        if(isWandering())
        {
            NextClusterNode();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isWandering())
        {
            Wander();
        }
        else if (isFleeing())
        {
            Flee();
        }
        else if (isFrozen())
        {
            Freeze();
        }
        
    }

    /**
     * Find a random neighbour and go visit that one
     */ 
    void Wander()
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

    /*
     * keep checking for the closest pursuer and set the state */
    void CheckNearbyPursuers()
    {

        float closestDist = float.MaxValue;
        foreach(PathfindingChaser pursuer in pursuers)
        {
            float dist = (pursuer.transform.position - transform.position).magnitude;
            if(dist < closestDist)
            {
                closestDist = dist;
                currentPursuer = pursuer;
            }

        }
        if(isWandering())
        {
            if(hasLineOfSight(currentPursuer) && closestDist < 3.0f)
            {
                currentState = State.FLEE;
            }
            
        }
        else if (isFleeing())
        {
            if(!hasLineOfSight(currentPursuer) && closestDist > 3.0f)
            {
                currentState = State.WANDER;
                CheckNearbyPursuers();
            }
        }
    }

    // same as pathfinding seek
    private void Flee()
    {
        Pathnode nextTarget = lastVisitedNode.neighbourNodes[0];
        
        //just for debug purposes
        foreach(Pathnode neighbour in lastVisitedNode.neighbourNodes)
        {
            Vector3 lookAtEnemyDirection = Vector3.Normalize(currentPursuer.transform.position - transform.position);
            Quaternion lookAtEnemyRotation = Quaternion.LookRotation(lookAtEnemyDirection);

            Vector3 lookAtNextNodeDirection = Vector3.Normalize(neighbour.position - transform.position);
            Quaternion lookAtNextNodeRotation = Quaternion.LookRotation(lookAtNextNodeDirection);

            if(Mathf.Abs(lookAtNextNodeRotation.eulerAngles.y - lookAtEnemyRotation.eulerAngles.y) > 90)
            {
                nextTarget = neighbour;
            }

        }
        Debug.DrawLine(transform.position, nextTarget.position, Color.blue);
        
        
        Vector3 vectorFromPursuer = currentPursuer.transform.position - transform.position;
        Vector3 pursuerDistVector = Vector3.Normalize(vectorFromPursuer);
        Vector3 destination = transform.position + -pursuerDistVector * 3f;
        //Pathnode nextTarget = Graph.GetNodeClosestTo(destination);

        Vector3 accelVector = Vector3.zero;

        Vector3 distanceVector = followedNode.position - transform.position;
        Vector3 velocityDirection = Vector3.Normalize(distanceVector);

        accelVector = velocityDirection * maxAccel;
        velocity = accelVector * Time.deltaTime;
        
        transform.LookAt(followedNode.position);

        if(distanceVector.magnitude < 0.1f && pathToFollow.hasNextNode())
        {
            lastVisitedNode = followedNode;
            followedNode = pathToFollow.nextNode();
        }
        if(distanceVector.magnitude < 0.05f)
        {
            lastVisitedNode = followedNode;
            velocity = Vector3.zero;
        }

        transform.position += velocity;

        float distFromPursuer = vectorFromPursuer.magnitude;
        if(distFromPursuer < 0.25f)
        {
            currentState = State.FROZEN;
        }
        

    }

    // refreshes the target node 
    private void Flee2()
    {
        // looks for a node in the opposite direction in the neighbour nodes, if none are found, default to the first neighbour
        if(isFleeing())
        {
            Pathnode nextTarget = lastVisitedNode.neighbourNodes[0];
            foreach(Pathnode neighbour in lastVisitedNode.neighbourNodes)
            {
                Vector3 lookAtEnemyDirection = Vector3.Normalize(currentPursuer.transform.position - transform.position);
                Quaternion lookAtEnemyRotation = Quaternion.LookRotation(lookAtEnemyDirection);

                Vector3 lookAtNextNodeDirection = Vector3.Normalize(neighbour.position - transform.position);
                Quaternion lookAtNextNodeRotation = Quaternion.LookRotation(lookAtNextNodeDirection);

                if(Mathf.Abs(lookAtNextNodeRotation.eulerAngles.y - lookAtEnemyRotation.eulerAngles.y) > 90)
                {
                    nextTarget = neighbour;
                }

            }
            Debug.DrawLine(transform.position, nextTarget.position, Color.blue);
            pathToFollow = Graph.FindShortestPath(transform.position, nextTarget.position);
            followedNode = pathToFollow.firstNode();

            // avoid backtracking
            if(followedNode.Equals(lastVisitedNode))
            {
                followedNode = pathToFollow.nextNode();
            }
        }

    }


    private bool hasLineOfSight(PathfindingChaser pursuer)
    {
        if(Physics.Linecast(transform.position, pursuer.transform.position))
        {
            return false;
        }
        return true;
    }
    private void Freeze()
    {
        //reset the game
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        transform.position = new Vector3(randomX, 0f, randomZ);
        currentState = State.WANDER;

        foreach(PathfindingChaser pursuer in pursuers)
        {
            pursuer.Reset();
        }
    }

    public bool isFrozen()
    {
        return currentState == State.FROZEN;
    }

    public bool isFleeing()
    {
        return currentState == State.FLEE;
    }

    public bool isWandering()
    {
        return currentState == State.WANDER;
    }
}
