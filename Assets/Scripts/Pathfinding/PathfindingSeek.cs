using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingSeek : MonoBehaviour
{
    public Transform target;
    public float maxAccel;
    public Vector3 velocity;
    public Pathnode followedNode;
    private Path pathToFollow;
    private Pathnode lastVisitedNode;
    

    // Start is called before the first frame update
    void Start()
    {
        CalculatePath();
        //InvokeRepeating("CalculatePath", 1.0f, 1.0f);
    }

    // seek target
    void FixedUpdate()
    {
        CalculatePath();

        Vector3 accelVector = Vector3.zero;
        
        Vector3 distanceVector = followedNode.position - transform.position;
        Vector3 velocityDirection = Vector3.Normalize(distanceVector);

        accelVector = velocityDirection * maxAccel;
        velocity = accelVector * Time.deltaTime;
        
        transform.LookAt(followedNode.position);

        // when we hit a node on our path and there is still more
        if(distanceVector.magnitude < 0.1f && pathToFollow.hasNextNode())
        {
            lastVisitedNode = followedNode;
            followedNode = pathToFollow.nextNode();
        }
        // when we hit the end of our path, stop
        if(distanceVector.magnitude < 0.05f)
        {
            lastVisitedNode = followedNode;
            velocity = Vector3.zero;
        }

        transform.position += velocity;

    }

    // find path to target, while avoiding backtracking to the previous node
    // backtracking is a result of the path generation algorithm
    void CalculatePath()
    {
        pathToFollow = Graph.FindShortestPath(transform.position, target.position);
        followedNode = pathToFollow.firstNode();

        if(followedNode.Equals(lastVisitedNode))
        {
            followedNode = pathToFollow.nextNode();
        }
    }

}
