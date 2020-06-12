using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshWander : MonoBehaviour
{
    private Bounds bounds;
    private float wanderTimer = 0.0f;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;
        agent = GetComponent<NavMeshAgent>();
    }

    /**
     * Randomizes a new destination to travel to every 2 seconds
     */
    void FixedUpdate()
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        if(Time.time > wanderTimer)
        {
            wanderTimer += 2.0f;
            agent.SetDestination(new Vector3(randomX, 0f, randomZ));
        }

    }
}
