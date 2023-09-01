using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to check where the forward vector is on the parent gameobject
/// since sometimes importing models can fuck up the axis
/// </summary>




public class ForwardVectorCheck : MonoBehaviour
{    
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 2.5f, Color.red);
    }
}
