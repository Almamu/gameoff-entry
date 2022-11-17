using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovementState : BossState
{
    /// <summary>
    /// The movement speed of the boss
    /// </summary>
    public float MovementSpeed = 4.0f;
    /// <summary>
    /// Factor used to control the rotation speed when in attack mode
    /// </summary>
    public float TurnFactor = 3.0f;
    /// <summary>
    /// Special distance to the arena's center used to rotate birds quicker
    /// </summary>
    public float AggresiveTurnDistance = 100.0f;
    
    void FixedUpdate ()
    {
        float turnSpeed = 0.02f;
        // first lerp the rotation to the player
        Transform t = this.transform;
        Vector3 forwardDirection = transform.forward;

        // move forward
        this.transform.Translate (Time.fixedDeltaTime * this.MovementSpeed * forwardDirection, Space.World);
        
        // calculate new rotation
        Vector3 finalDirection = this.Machine.Objective.transform.position - t.position;
        
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
}
