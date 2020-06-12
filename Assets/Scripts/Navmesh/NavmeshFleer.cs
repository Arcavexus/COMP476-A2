using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshFleer : MonoBehaviour
{
    public enum State { WANDER, FLEE, FROZEN}
    public State currentState;
    public Vector3 targetLocation;
    public List<GameObject> pursuers;
    public GameObject currentPursuer;
    private NavMeshAgent agent;
    private Bounds bounds;
    private float wanderTimer = 0.0f;

    public float fleeRadius = 2.0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;
    }

    void FixedUpdate()
    {
        float closestDist = float.MaxValue;
        
        // get the closest pursuer to flee away from
        foreach(GameObject pursuer in pursuers)
        {
            float distance = (pursuer.transform.position - transform.position).magnitude;
            if(distance < closestDist)
            {
                closestDist = distance;
                currentPursuer = pursuer;
            }
        }

        // if you lose sight of the current pursuer and there is no one in our fleeRadius, wander again
        if(!hasLineOfSight(currentPursuer) && closestDist > fleeRadius)
        {
            currentState = State.WANDER;
        }

        // standard FSM
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
     * Randomizes a new destination to travel to every 2 seconds
     */
    void Wander()
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        if(Time.time > wanderTimer)
        {
            wanderTimer += 2.0f;
            agent.SetDestination(new Vector3(randomX, 0f, randomZ));
        }

        if(hasLineOfSight(currentPursuer))
        {
            currentState = State.FLEE;
        }

    }

    /**
     * Flee has the car flee in a random direction away from the pursuer
     * We keep track of two points to the left and right of the car to check if those points are available in
     * case the car gets stuck in a corner
     */
    void Flee()
    {

        Vector3 awayVector = Vector3.Normalize(transform.position - currentPursuer.transform.position);
        Vector3 rightVector = transform.position + transform.right;
        Vector3 leftVector = transform.position + -transform.right;

        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * 0.2f;
        Vector3 randomPoint = transform.position + awayVector * 1.5f + new Vector3(randomCirclePoint.x, 0, randomCirclePoint.y);
        
        

        Debug.DrawLine(transform.position, randomPoint, Color.blue);
        Debug.DrawLine(transform.position, rightVector, Color.blue);
        Debug.DrawLine(transform.position, leftVector, Color.blue);

        float dist = Vector3.Distance(transform.position, currentPursuer.transform.position);

        if(dist < fleeRadius)
        {
            if(Mathf.Abs(randomPoint.x) > bounds.max.x && Mathf.Abs(randomPoint.z) > bounds.max.z)
            {
                if(rightVector.x < bounds.max.x && rightVector.z  < bounds.max.z
                    && rightVector.x > bounds.min.x && rightVector.z > bounds.min.z)
                {
                    agent.SetDestination(rightVector);
                }
                else if (leftVector.x < bounds.max.x && leftVector.z < bounds.max.z
                    && leftVector.x > bounds.min.x && leftVector.z > bounds.min.z)
                {
                    agent.SetDestination(leftVector);
                }
                    
            }
            else
            {
                agent.SetDestination(randomPoint);
            }
            
            if(dist < 0.25f)
            {
                currentState = State.FROZEN;
            }
        }
    }

    /**
     * When the fleer gets frozen, reset the game and assign it to a new position
     */
    void Freeze()
    {
        //reset the game
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        transform.position = new Vector3(randomX, 0f, randomZ);
        currentState = State.WANDER;

        foreach(GameObject pursuer in pursuers)
        {
            pursuer.GetComponent<NavmeshSeek>().Reset();
        }
    }

    private bool hasLineOfSight(GameObject pursuer)
    {
        if(Physics.Linecast(transform.position, pursuer.transform.position))
        {
            return false;
        }
        return true;
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
