using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdMovement : BirdState
{
    /// <summary>
    /// The percentage of probability a bird will attack the player
    /// </summary>
    public int AttackProbability = 30;

    /// <summary>
    /// The maximum amount of time a bird will spend flying before the next state change
    /// </summary>
    public float MaximumTimeFlying = 5.0f;
    
    /// <summary>
    /// The area where the bird can move around
    /// </summary>
    private BoxCollider mMovementArea;
    
    /// <summary>
    /// The point the bird is moving to
    /// </summary>
    private Vector3 mDestination;

    /// <summary>
    /// The time spent on the current movement
    /// </summary>
    private float mMovementTime = 0.0f;

    // this only has to happen once
    void Start()
    {
        this.mMovementArea = this.transform.parent.Find ("MovementArea").GetComponent <BoxCollider> ();
    }

    /// <summary>
    /// Gets a random point inside the movement area
    /// </summary>
    /// <returns></returns>
    private Vector3 PickRandomPoint ()
    {
        Bounds bounds = this.mMovementArea.bounds;
        
        return new Vector3 (
            Random.Range (bounds.min.x, bounds.max.x),
            this.transform.position.y,
            Random.Range (bounds.min.z, bounds.max.z)
        );
    }
    
    public override void OnStateEnter ()
    {
        // search for a destination
        this.mDestination = this.PickRandomPoint ();
        // reset movement timer
        this.mMovementTime = 0.0f;
    }

    private void HandleMovement ()
    {
        // get the direction
        Vector3 finalDirection = this.transform.position - this.mDestination;
        finalDirection.Normalize ();
        
        // get the forward vector and lerp both
        finalDirection = math.lerp (this.transform.forward, finalDirection, 1.0f);
        
        // translate the bird in the right direction
        this.transform.Translate (Time.fixedDeltaTime * 5 * finalDirection);
        this.transform.rotation = Quaternion.LookRotation (finalDirection);
    }

    private void CheckDistance ()
    {
        // calculate distante
        float distance = (this.transform.position - this.mDestination).magnitude;

        // TODO: PROPERLY MEASURE A GOOD MAGNITUDE
        if (distance > 15.0f || this.mMovementTime < this.MaximumTimeFlying)
            return;

        // select next step
        int percent = Random.Range (0, 100);

        if (percent < this.AttackProbability)
        {
            // push attack state
            this.Machine.PushState (this.AttackState);
        }
        else
        {
            // picks a new point to move to
            this.OnStateEnter ();
        }
    }

    void FixedUpdate ()
    {
        this.HandleMovement ();
        this.CheckDistance ();

        this.mMovementTime += Time.fixedDeltaTime;
    }

    private void OnDrawGizmos ()
    {
        // draw a ball at the destination point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (this.mDestination, 0.5f);
    }
}
