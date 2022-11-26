using UnityEngine;

public class PlayerDodge : PlayerState
{
    [HideInInspector]
    public Vector3 Direction { get; set; }

    /// <summary>
    /// Indicates the strength of the dodge movement
    /// </summary>
    public float Strength = 8.0f;

    /// <summary>
    /// Timer that indicates how long the player will stay in this state
    /// </summary>
    public float InvulnerabilityTimer = 0.7f;

    /// <summary>
    /// Current status of the timer
    /// </summary>
    private float mCurrentTime = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        this.mCurrentTime += Time.deltaTime * Time.timeScale;

        if (this.mCurrentTime > this.InvulnerabilityTimer)
            this.Machine.PopState();
    }

    public override void OnStateEnter()
    {
        this.Rigidbody.velocity = new Vector3 (
            this.Direction.x * Strength,
            this.Rigidbody.velocity.y,
            this.Direction.z * Strength
        );
        
        // make the character look at the direction we're jumping so it looks a bit more natural
        transform.rotation = Quaternion.LookRotation (this.Direction);

        // reset the timer
        this.mCurrentTime = 0.0f;
        // disable collision
        Physics.IgnoreLayerCollision (PlayerStateMachine.PlayerCollisionLayer, PlayerStateMachine.BirdsCollisionLayer, true);
        Physics.IgnoreLayerCollision (PlayerStateMachine.PlayerCollisionLayer, PlayerStateMachine.FlyingAttacksLayer, true);
        // set the dodge boolean so any animation is interrupted
        this.Machine.ModelAnimator.SetBool (PlayerStateMachine.WalkingId, false);
        this.Machine.ModelAnimator.SetBool (PlayerStateMachine.DodgeId, true);
    }

    public override void OnStateExit ()
    {
        // reset the dodge boolean
        this.Machine.ModelAnimator.SetBool (PlayerStateMachine.DodgeId, false);
        
        if (this.Machine.IsInvulnerable () == true)
            return;
        
        // set collision layers so birds can hit the player again
        Physics.IgnoreLayerCollision (PlayerStateMachine.PlayerCollisionLayer, PlayerStateMachine.BirdsCollisionLayer, false);
        Physics.IgnoreLayerCollision (PlayerStateMachine.PlayerCollisionLayer, PlayerStateMachine.FlyingAttacksLayer, false);
    }
}
