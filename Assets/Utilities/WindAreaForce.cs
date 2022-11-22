using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAreaForce : MonoBehaviour
{
    /// <summary>
    /// The collider for the wind area
    /// </summary>
    public SphereCollider Collider { get; set; }
    
    /// <summary>
    /// Force multiplier for the wind
    /// </summary>
    public float Multiplier { get; set; }

    void Awake ()
    {
        this.Collider = GetComponent <SphereCollider> ();
    }
}
