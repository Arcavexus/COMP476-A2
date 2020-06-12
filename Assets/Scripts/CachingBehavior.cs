using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Coroutine = System.Collections.IEnumerator;

public class CachingBehavior : MonoBehaviour 
{
    private Transform _Transform;
    public new Transform transform
    {
        get
        {
            if (_Transform == null)
                _Transform = base.transform;
            return _Transform;
        }
    }

    private Rigidbody _Rigidbody;
    public new Rigidbody rigidbody
    {
        get
        {
            if (_Rigidbody == null)
                _Rigidbody = base.GetComponent<Rigidbody>();
            return _Rigidbody;
        }
    }

    private Renderer _Renderer;
    public new Renderer renderer
    {
        get
        {
            if (_Renderer == null)
                _Renderer = base.GetComponent<Renderer>();
            return _Renderer;
        }
    }

    private Collider _Collider;
    public new Collider collider
    {
        get
        {
            if (_Collider == null)
                _Collider = base.GetComponent<Collider>();
            return _Collider;
        }
    }
}
