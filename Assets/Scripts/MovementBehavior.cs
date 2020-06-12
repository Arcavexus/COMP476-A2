using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;

public class MovementBehavior : CachingBehavior
{
    protected Bounds terrainBounds;

	public virtual void Start () 
	{
        terrainBounds = Terrain.activeTerrain.GetComponent<Collider>().bounds;
	}

    protected Vector3 WrapPosition(Vector3 pos)
    {
        if (pos.x > terrainBounds.max.x)
            pos.x -= terrainBounds.size.x;
        else if (pos.x < terrainBounds.min.x)
            pos.x += terrainBounds.size.x;

        if (pos.z > terrainBounds.max.z)
            pos.z -= terrainBounds.size.z;
        else if (pos.z < terrainBounds.min.z)
            pos.z += terrainBounds.size.z;

        return pos;
    }

    protected void MoveBy(Vector3 disp)
    {
        Vector3 newPos = transform.position + disp;

        newPos = WrapPosition(newPos);

        transform.position = newPos;
    }

    protected Vector3 DisplacementVector(Vector3 source, Vector3 target)
    {
        Vector3 diff = target - source;
        diff.y = 0;

        if (Mathf.Abs(diff.x) > terrainBounds.extents.x)
            diff.x = -diff.x;

        if (Mathf.Abs(diff.z) > terrainBounds.extents.z)
            diff.z = -diff.z;

        return diff;
    }
}
