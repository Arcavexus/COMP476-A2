using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

[RequireComponent(typeof(PathfindingSeek))]
[RequireComponent(typeof(PathfindingWander))]
public class PathfindingChaser : CachingBehavior 
{
    private PathfindingWander pathfindingWander;
    private PathfindingSeek pathfindingSeek;
    private Bounds bounds;

    public PathfindingFleer target;
    public Path currentlyFollowedPath;
    public bool Disabled { get; set; }


    private BehaviorTree bt;

    void Awake()
    {
        pathfindingSeek = GetComponent<PathfindingSeek>();
        pathfindingWander = GetComponent<PathfindingWander>();

        Wander();
        bounds = GameObject.Find("Plane").GetComponent<MeshCollider>().bounds;

        InitBT();
        bt.Start();
    }

    public void Wander()
    {
        pathfindingSeek.enabled = false;
        pathfindingWander.enabled = true;
        // pathfindingSeek.target = null;
    }

    public void ChaseTarget(Transform target)
    {
        pathfindingWander.enabled = false;
        pathfindingSeek.target = target;
        pathfindingSeek.enabled = true;
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
        if (!pathfindingWander.enabled)
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
