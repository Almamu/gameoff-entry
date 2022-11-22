using UnityEngine;

public class BossMovementState : BossState
{
    /// <summary>
    /// The movement speed of the boss
    /// </summary>
    public float MovementSpeed = 4.0f;

    /// <summary>
    /// The speed at which the boss will look at the player
    /// </summary>
    public float RotationSpeed = 1.0f;
    
    void FixedUpdate ()
    {
        // rotate the parent
        transform.parent.Rotate (Vector3.up, this.MovementSpeed * Time.fixedDeltaTime);

        Vector3 playerDirection = this.Machine.Objective.transform.position - transform.position;

        playerDirection = Vector3.Lerp (transform.forward, playerDirection.normalized, this.RotationSpeed);

        playerDirection.y = 0.0f;
        
        transform.rotation = Quaternion.LookRotation (playerDirection);
    }
}
