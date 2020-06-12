using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshSeek : MonoBehaviour
{
    
    public Transform target;
    private NavMeshAgent agent;
    private Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;
    }

    void Update()
    {
        // debug purposes
        if (Input.GetMouseButtonDown(0))
        {
            SetDestinationToMousePosition();
        }

        agent.SetDestination(target.position);
    }

    void SetDestinationToMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            agent.SetDestination(hit.point);
        }
    }

    public void Reset()
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        transform.position = new Vector3(randomX, 0f, randomZ);
    }
}
