using Extensions;
using Unity.Mathematics;
using UnityEngine;

public class BossWingsAttack : BossState
{
    /// <summary>
    /// The duration of the attack
    /// </summary>
    public float Duration = 5.0f;

    /// <summary>
    /// The difference of size between the highest point and lowest point of the wave
    /// </summary>
    public float Difference = 2.0f;

    /// <summary>
    /// The speed at which the wind collider grows and shrinks
    /// </summary>
    public float Speed = 12.0f;

    /// <summary>
    /// The force multiplier for the wind effect
    /// </summary>
    public float Force = 2.0f;

    /// <summary>
    /// The current timer
    /// </summary>
    private float mTimer = 0.0f;

    /// <summary>
    /// The collider used to specify the wind's area
    /// </summary>
    public WindAreaForce WindAreaForce;

    private float mDefaultRadius;

    public override void OnStateEnter ()
    {
        // sets the multiplier
        this.WindAreaForce.Multiplier = this.Force;
        this.mTimer = this.Duration;
        this.WindAreaForce.gameObject.SetActive (true);
        this.mDefaultRadius = this.WindAreaForce.Collider.radius;
        
        this.Machine.ModelAnimator.SetBool (BossStateMachine.WingsHash, true);
    }

    public override void OnStateExit ()
    {
        this.WindAreaForce.Collider.radius = this.mDefaultRadius;
        this.WindAreaForce.gameObject.SetActive (false);
        
        this.Machine.ModelAnimator.SetBool (BossStateMachine.WingsHash, false);
    }

    void FixedUpdate ()
    {
        this.mTimer -= Time.fixedDeltaTime;
        
        if (this.mTimer < 0.0f)
            this.Machine.PopState ();

        this.WindAreaForce.Collider.radius = this.mDefaultRadius + (math.cos (Time.fixedTime * this.Speed) + this.Difference);
    }
}
