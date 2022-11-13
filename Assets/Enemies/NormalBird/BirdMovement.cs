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
    /// The minimum amount of time a bird will spend flying before the next state change
    /// </summary>
    public float MinimumTimeFlying = 1.0f;
    
    /// <summary>
    /// The maximum amount of time a bird will spend flying before the next state change
    /// </summary>
    public float MaximumTimeFlying = 4.0f;

    /// <summary>
    /// The speed to rotate towards the final point
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float TurnSpeed = 0.01f;

    /// <summary>
    /// Special distance to the arena's center used to rotate birds quicker
    /// </summary>
    public float AggresiveTurnDistance = 100.0f;
    
    /// <summary>
    /// The speed at which the bird will move
    ///
    /// TODO: MAYBE USE THIS AS RANGE AND RANDOMIZE IT FOR DIFFERENT BIRDS?
    /// </summary>
    public float MovementSpeed = 5.0f;

    /// <summary>
    /// The point the bird is moving to
    /// </summary>
    private Vector3 mDestination;

    /// <summary>
    /// The time spent on the current movement
    /// </summary>
    private float mMovementTime = 1.0f;

    /// <summary>
    /// Gets a random point inside the movement area
    /// </summary>
    /// <returns></returns>
    private Vector3 PickRandomPoint ()
    {
        Bounds bounds = this.Machine.MovementArea.bounds;
        
        return new Vector3 (
            Random.Range (bounds.min.x, bounds.max.x),
            this.Machine.MovementArea.transform.position.y,
            Random.Range (bounds.min.z, bounds.max.z)
        );
    }
    
    public override void OnStateEnter ()
    {
        // search for a destination
        this.mDestination = this.PickRandomPoint ();
        // reset movement timer
        this.mMovementTime = Random.Range (this.MinimumTimeFlying, this.MaximumTimeFlying);
    }

    private void HandleMovement ()
    {
        float turnSpeed = this.TurnSpeed;
        Transform t = this.transform;
        Vector3 forwardDirection = t.forward;
        
        // move forward
        this.transform.Translate (Time.fixedDeltaTime * MovementSpeed * forwardDirection, Space.World);
        
        // get the direction
        Vector3 finalDirection = this.mDestination - t.position;
        
        // calculate distance to the destination and do an aggressive turn if outside
        if (finalDirection.sqrMagnitude > this.AggresiveTurnDistance)
            turnSpeed = 0.05f;
            
        // get the forward vector and lerp both
        finalDirection = Vector3.Lerp (forwardDirection, finalDirection.normalized, turnSpeed);

        // do not move vertically
        finalDirection.y = 0.0f;
        
        // translate the bird in the right direction
        this.transform.rotation = Quaternion.LookRotation (finalDirection);
    }

    private void CheckDistance ()
    {
        // calculate distance
        float distance = (this.mDestination - this.transform.position).magnitude;

        // TODO: PROPERLY MEASURE A GOOD MAGNITUDE
        if (distance > 15.0f && this.mMovementTime > 0.0f)
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

        this.mMovementTime -= Time.fixedDeltaTime;
    }

    private void OnDrawGizmos ()
    {
        // do not draw gizmos if disabled
        if (this.enabled == false)
            return;
        
        Vector3 position = transform.position;
        
        // draw a ball at the destination point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (this.mDestination, 0.5f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine (position, this.mDestination);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay (position, transform.forward);
    }
}
