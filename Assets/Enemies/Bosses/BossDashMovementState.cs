using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDashMovementState : BossMovementState
{
    /// <summary>
    /// The amount of time the dash lasts
    /// </summary>
    public float DashDuration = 1.0f;

    void FixedUpdate ()
    {
        this.DashDuration -= Time.fixedDeltaTime;
        
        if (this.DashDuration <= 0.0f)
            this.Machine.PopState ();
    }
}
