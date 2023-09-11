using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionData
{
    public Rigidbody rb;
    public Rigidbody other;
    public Vector3 position;
    public Vector3 normal;
    public LayerMask objectFilter;


    public void Reset()
    {
        rb = null;
        other = null;
        position = Vector3.zero;
        normal = Vector3.zero;
        
    }
}



