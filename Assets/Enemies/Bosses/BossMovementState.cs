using Unity.Mathematics;
using UnityEngine;

public class BossMovementState : BossState
{
    /// <summary>
    /// The base movement speed for the boss
    /// </summary>
    public float BaseMovementSpeed = 4.0f;
    /// <summary>
    /// The final movement speed for the boss when close to the switch threshold
    /// </summary>
    public float FinalMovementSpeed = 10.0f;
    
    private float mMovementSpeed = 0;

    void Start ()
    {
        CombatEventManager.BossHealth += this.BossHealthUpdate;
    }
    
    new void Awake ()
    {
        base.Awake ();

        this.mMovementSpeed = this.BaseMovementSpeed;
    }
    
    void FixedUpdate ()
    {
        if (this.Machine.Controller.Phase != BossPhase.First)
            return;
        
        // rotate the parent
        transform.parent.Rotate (Vector3.up, this.mMovementSpeed * Time.fixedDeltaTime);
        // look at the player
        transform.LookAt (this.Machine.Objective.transform);
    }

    void BossHealthUpdate (float newHealth)
    {
        // substract the threshold so we get a smooth transition between 100-50 within the life
        newHealth = newHealth - this.Machine.Controller.SecondPhaseHealthThreshold;
        float progress = newHealth / (this.Machine.Controller.Health - this.Machine.Controller.SecondPhaseHealthThreshold);
        
        this.mMovementSpeed = math.lerp (this.BaseMovementSpeed, this.FinalMovementSpeed, progress);
    }
}
