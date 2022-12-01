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
        // move forward
        transform.Translate (Vector3.forward * this.MovementSpeed);
        // get current position
        Vector3 currentPosition = transform.position;
        
        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;
        
        // end of the shooting, enable childs and destroy ourselves
        foreach (SmallRacimoAttackBehaviour child in this.mChilds)
        {
            Vector3 childPosition = child.transform.position;
            
            // store default information for reset purposes
            child.StartParent = transform;
            child.StartLocalPosition = child.transform.localPosition;
            // get the direction to the parent
            Vector3 direction = Vector3.Normalize(childPosition - currentPosition);
            // look in the opposite direction
            child.transform.LookAt (childPosition + direction);
            // parent it to our parent so they don't clutter the hierarchy
            child.transform.SetParent (transform.parent);
            // activate them
            child.gameObject.SetActive (true);
        }
        
        // reset timer
        this.mTimer = this.SeparateTimer;
        
        this.gameObject.SetActive (false);
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (this.enabled == false || collision.gameObject.CompareTag ("Player") == false)
            return;

        collision.gameObject.SendMessage ("ApplyBigRacimoDamage");
    }
}
