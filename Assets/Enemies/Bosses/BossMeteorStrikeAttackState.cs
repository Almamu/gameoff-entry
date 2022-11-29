using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BossMeteorStrikeAttackState : BossState
{
    /// <summary>
    /// The time it takes for the boss to fall down
    /// </summary>
    public float StrikeTime = 5.0f;
    /// <summary>
    /// The height were the boss will stay at until the attack is completed
    /// </summary>
    public float StrikeHeight = 10.0f;
    /// <summary>
    /// The speed to move at when falling down
    /// </summary>
    public float StrikeSpeed = 10.0f;
    /// <summary>
    /// Timer used for strike movement
    /// </summary>
    private float mTimer;
    /// <summary>
    /// The object used to display the strike effect
    /// </summary>
    public Renderer StrikeEffect;
    /// <summary>
    /// The default layer mask to check collision for
    /// </summary>
    private static int DefaultLayerMask;
    /// <summary>
    /// The id of the progress variable for the material
    /// </summary>
    private static int ProgressID = Shader.PropertyToID ("_Progress");
    /// <summary>
    /// The destination point for the boss when falling down
    /// </summary>
    private Vector3 mDestination;
    new void Awake ()
    {
        base.Awake ();
        
        this.StrikeEffect.transform.parent = null;
        DefaultLayerMask = LayerMask.GetMask ("Default");
    }
    
    public override void OnStateEnter ()
    {
        this.mTimer = this.StrikeTime;
        
        // create the object below the target
        Vector3 position = this.Machine.Objective.transform.position;

        // check for ground, no match means the attack cannot be performed for whatever reason
        if (Physics.Raycast (position, Vector3.down, out RaycastHit hit, math.INFINITY, DefaultLayerMask) == false)
            this.Machine.PopState ();

        this.StrikeEffect.transform.position = hit.point;
        this.StrikeEffect.gameObject.SetActive (true);
        
        // decide the point at which the boss should stop moving
        this.mDestination = new Vector3 (
            hit.point.x,
            transform.position.y,
            hit.point.z
        );
        // move the boss to the sky so it's not visible
        this.transform.position = hit.point + Vector3.up * this.StrikeHeight;
        this.Machine.ModelAnimator.SetBool (BossStateMachine.StrikeHash, true);
    }

    public override void OnStateExit ()
    {
        // play the particles
        this.Machine.MeteorStrikeParticles.Play ();
        // deactivate the strike area
        this.mTimer = 0.0f;
        this.StrikeEffect.gameObject.SetActive (false);
        this.Machine.ModelAnimator.SetBool (BossStateMachine.StrikeHash, false);
    }

    void FixedUpdate ()
    {
        this.mTimer -= Time.fixedDeltaTime;

        // update the shadow so it reflects the right progress
        this.StrikeEffect.material.SetFloat (ProgressID, (this.StrikeTime - this.mTimer) / this.StrikeTime);
        
        if (this.mTimer > 0.0f)
            return;

        Transform t = transform;
        float movementDistance = Time.fixedDeltaTime * this.StrikeSpeed;
        
        // calculate distance and pop the state if the boss is already there
        if (Vector3.Distance (t.position, this.mDestination) < movementDistance)
        {
            this.Machine.PopState ();
            
            // ensure the position is right to prevent slight variance
            transform.position = this.mDestination;
            
            return;
        }
        
        // move the boss quickly towards the point
        transform.position = Vector3.MoveTowards (t.position, this.mDestination, movementDistance);
    }
}
