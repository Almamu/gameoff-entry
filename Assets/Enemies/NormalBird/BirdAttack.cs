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
    public float MinimumAttackSpeed = 0.0f;
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
    /// The current movement speed
    /// </summary>
    private float mMovementSpeed = 0.0f;
    
    public override void OnStateEnter ()
    {
        // decide the movement speed
        this.mMovementSpeed = Random.Range (this.MinimumAttackSpeed, this.MaximumAttackSpeed);
    }

    void FixedUpdate ()
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

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.CompareTag ("Player") == false || this.enabled == false)
            return;
        
        // bird collided with the player, we can go back to normal movement
        this.Machine.PopState ();
    }
}
