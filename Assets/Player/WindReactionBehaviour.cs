using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WindReactionBehaviour : MonoBehaviour
{
    /// <summary>
    /// Whether the object is inside a wind zone or not
    /// </summary>
    private bool mInWindZone = false;

    /// <summary>
    /// The zone that is applying wind right now
    /// </summary>
    private WindAreaForce mWindOrigin;

    /// <summary>
    /// The rigidbody that will receive the wind force
    /// </summary>
    private Rigidbody mRigidbody;
    
    void Start ()
    {
        this.mRigidbody = GetComponent <Rigidbody> ();
    }

    void FixedUpdate ()
    {
        if (this.mInWindZone == false)
            return;
        
        // get direction
        Vector3 direction = transform.position - this.mWindOrigin.transform.position;
        // some assumptions made here
        // the sphere is regular and scaled the same way in every axis
        // if that wasn't the case, this operation wouldn't work at all
        float radiusInUnits = this.mWindOrigin.Collider.radius * this.mWindOrigin.Collider.transform.lossyScale.x;
        float distance = direction.magnitude;
        float force = radiusInUnits - distance;
        
        // apply multiplier with a lerp, that way it can scale linearly
        force = math.lerp (force, force * this.mWindOrigin.Multiplier, distance / radiusInUnits);

        // normalize the difference so it can be applied as force
        direction.Normalize ();

        // do not apply negative forces
        if (force < 0.0f)
            return;
        
        // based on distance, apply different forces
        if (distance < 5.0f)
        {
            // being too close should apply some knockback
            force *= 5.0f;
            // also some damage if the object supports it
            this.SendMessage ("ApplyWindDamage");
        }            
        
        // apply it to the rigidbody
        this.mRigidbody.AddForce (force * direction, ForceMode.Impulse);
    }
    
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag ("Wind effect") == false || other.gameObject.activeSelf == false)
            return;

        this.mWindOrigin = other.gameObject.GetComponent <WindAreaForce> ();
        this.mInWindZone = true;
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.CompareTag ("Wind effect") == false)
            return;

        this.mWindOrigin = null;
        this.mInWindZone = false;
    }
}
