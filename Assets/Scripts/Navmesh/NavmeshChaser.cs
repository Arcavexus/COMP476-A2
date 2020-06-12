using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

[RequireComponent(typeof(NavmeshSeek))]
[RequireComponent(typeof(NavmeshWander))]
public class NavmeshChaser : CachingBehavior 
{
    private NavmeshWander navmeshWander;
    private NavmeshSeek navmeshSeek;
    private Bounds bounds;
    private BehaviorTree bt;

    public NavmeshFleer target;
    public Path currentlyFollowedPath;
    public bool Disabled { get; set; }


    void Awake()
    {
        navmeshSeek = GetComponent<NavmeshSeek>();
        navmeshWander = GetComponent<NavmeshWander>();

        Wander();
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;

        InitBT();
        bt.Start();
    }

    public void Wander()
    {
        navmeshSeek.enabled = false;
        navmeshWander.enabled = true;
    }

    // set target on seek script
    public void ChaseTarget(Transform target)
    {
        navmeshWander.enabled = false;
        navmeshSeek.target = target;
        navmeshSeek.enabled = true;
    }

    private void InitBT()
    {
        bt = new BehaviorTree(Application.dataPath + "/car-behavior.xml", this);
    }

    [BTLeaf("is-frozen")]
    public bool IsFrozen()
    {
        return Disabled;
    }

    [BTLeaf("has-line-of-sight")]
    public bool HasLineOfSight()
    {
        if(Physics.Linecast(transform.position, target.transform.position))
        {
            return false;
        }
        return true;
    }

    
    [BTLeaf("is-closest-to-intruder")]
    public bool IsClosestToIntruder()
    {
        return false;
    }

    [BTLeaf("chase-target")]
    public BTCoroutine SeekTarget()
    {
        ChaseTarget(target.transform);
        while (true)
        {
            if (target.isFrozen())
            {
                yield return BTNodeResult.Success;
                yield break;
            }
            else if (!HasLineOfSight())
            {
                yield return BTNodeResult.Failure;
                yield break;
            }

            yield return BTNodeResult.NotFinished;
        }
    }

    [BTLeaf("wander")]  
    public BTCoroutine WanderRoutine()
    {
        if (!navmeshWander.enabled)
        {
            Wander();
        }
        yield return BTNodeResult.Success;
    }

    public void Reset()
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        transform.position = new Vector3(randomX, 0f, randomZ);
        Wander();
    }
}
