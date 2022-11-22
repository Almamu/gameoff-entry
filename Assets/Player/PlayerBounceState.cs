using UnityEditor;
using UnityEngine;

public class PlayerBounceState : PlayerState
{
    /// <summary>
    /// Timer that indicates the amount of time the player cannot move by himself
    /// </summary>
    public float BounceTimer = 0.7f;

    /// <summary>
    /// Current status of the timer
    /// </summary>
    private float mCurrentTime = 0.0f;

    public override void OnStateEnter ()
    {
        this.mCurrentTime = this.BounceTimer;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        this.mCurrentTime -= Time.fixedDeltaTime;

        if (this.mCurrentTime > 0.0f)
            return;
        
        this.Machine.PopState ();
    }
}
