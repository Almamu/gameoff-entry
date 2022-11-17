using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdAttack : BirdState
{
    /// <summary>
    /// The minimum movement speed for an attack
    /// </summary>
    public float MinimumAttackSpeed = 4.0f;
    /// <summary>
    /// The maximum movement speed for an attack
    /// </summary>
    public float MaximumAttackSpeed = 10.0f;
    /// <summary>
    /// Factor used to control the rotation speed when in attack mode
    /// </summary>
    public float TurnFactor = 1.0f;
    /// <summary>
    /// Special distance to the arena's center used to rotate birds quicker
    /// </summary>
    public float AggresiveTurnDistance = 100.0f;
    /// <summary>
    /// The maximum time (in seconds) the bird can be in attack mode
    /// </summary>
    public float MaxAttackModeTime = 10.0f;
    /// <summary>
    /// The current movement speed
    /// </summary>
    private float mMovementSpeed = 0.0f;
    /// <summary>
    /// Indicates whether the bird got into the close aggressiveness area of the player
    /// </summary>
    private bool mInAggressivenessArea = false;
    /// <summary>
    /// The time the bird has been in attack mode
    /// </summary>
    private float mCurrentTime = 0.0f;
    
    public override void OnStateEnter ()
    {
        // decide the movement speed
        this.mInAggressivenessArea = false;
        this.mMovementSpeed = Random.Range (this.MinimumAttackSpeed, this.MaximumAttackSpeed);
        this.mCurrentTime = 0.0f;
    }

    private void HandleMovement ()
    {
        float turnSpeed = 0.02f;
        // first lerp the rotation to the player
        Transform t = this.transform;
        Vector3 forwardDirection = transform.forward;

        // move forward
        this.transform.Translate (Time.fixedDeltaTime * this.mMovementSpeed * forwardDirection, Space.World);
        
        // calculate new rotation
        Vector3 finalDirection = this.Machine.Player.transform.position - t.position;
        
        // calculate distance to the destination and do an aggressive turn if outside
        if (finalDirection.sqrMagnitude > this.AggresiveTurnDistance)
            turnSpeed = 0.05f;
            
        // get the forward vector and lerp both
        finalDirection = Vector3.Lerp (forwardDirection, finalDirection.normalized, turnSpeed * this.TurnFactor);

        // do not move vertically
        finalDirection.y = 0.0f;
        
        // translate the bird in the right direction
        this.transform.rotation = Quaternion.LookRotation (finalDirection);
    }

    private void HandleAggressivenessArea ()
    {
        if (this.mInAggressivenessArea == false)
            return;
    }

    private void HandleTimer ()
    {
        this.mCurrentTime += Time.fixedDeltaTime;
        
        if (this.mCurrentTime > this.MaxAttackModeTime)
            this.Machine.PopState ();
    }
    
    void FixedUpdate ()
    {
        this.HandleMovement ();
        this.HandleAggressivenessArea ();
        this.HandleTimer ();
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (this.enabled == false || collision.gameObject.CompareTag ("Player") == false)
            return;
        
        // bird collided with the player, we can go back to normal movement
        this.Machine.PopState ();
    }

    private void OnTriggerEnter (Collider other)
    {
        if (this.enabled == false || other.CompareTag ("AggressivenessArea") == false)
            return;

        this.mInAggressivenessArea = true;
    }

    private void OnTriggerExit (Collider other)
    {
        if (this.enabled == false || this.mInAggressivenessArea == false || other.CompareTag ("AggressivenessArea") == false)
            return;

        // the bird almost hit the player but failed, go back to normal movement
        this.Machine.PopState ();
    }
}
