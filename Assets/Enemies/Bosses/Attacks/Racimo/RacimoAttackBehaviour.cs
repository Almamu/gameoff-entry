using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacimoAttackBehaviour : MonoBehaviour
{
    /// <summary>
    /// The time it takes for the racimo to spawn the smaller bullets
    /// </summary>
    public float SeparateTimer = 1.2f;
    /// <summary>
    /// The speed at which this attack moves
    /// </summary>
    public float MovementSpeed = 0.6f;

    /// <summary>
    /// The racimo small attacks that are spawned on destroyal of this one
    /// </summary>
    private SmallRacimoAttackBehaviour [] mChilds;
    /// <summary>
    /// The timer for creating the small racimo bullets
    /// </summary>
    private float mTimer = 0.0f;
    
    void Awake ()
    {
        this.mChilds = this.GetComponentsInChildren <SmallRacimoAttackBehaviour> (true);
        this.mTimer = this.SeparateTimer;
    }
    
    void FixedUpdate ()
    {
        Transform current = transform;
        
        // move forward
        current.position += current.forward * this.MovementSpeed;
        
        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;
        
        // end of the shooting, enable childs and destroy ourselves
        foreach (SmallRacimoAttackBehaviour child in this.mChilds)
        {
            // store default information for reset purposes
            child.StartParent = transform;
            child.StartLocalPosition = child.transform.localPosition;
            // get the direction to the parent
            Vector3 direction = child.transform.position - this.transform.position;
            // get it as a direction vector
            direction.Normalize ();
            // look in the opposite direction
            child.transform.LookAt (child.transform.position + direction);
            // parent it to our parent so they don't clutter the hierarchy
            child.transform.SetParent (transform.parent);
            // activate them
            child.gameObject.SetActive (true);
        }
        
        // reset timer
        this.mTimer = this.SeparateTimer;
        
        this.gameObject.SetActive (false);
    }
}