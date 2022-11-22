using UnityEngine;

public class BossMovementState : BossState
{
    /// <summary>
    /// The movement speed of the boss
    /// </summary>
    public float MovementSpeed = 4.0f;

    void FixedUpdate ()
    {
        // rotate the parent
        transform.parent.Rotate (Vector3.up, this.MovementSpeed * Time.fixedDeltaTime);
        // look at the player
        transform.LookAt (this.Machine.Objective.transform);
    }
}
