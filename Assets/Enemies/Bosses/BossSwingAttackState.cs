using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwingAttackState : BossState
{
    /// <summary>
    /// The increments for each launch
    /// </summary>
    public float RotationIncrements = 6.0f;
    /// <summary>
    /// The amount of times to shoot
    /// </summary>
    public int NumberOfBullets = 5;
    /// <summary>
    /// The time it takes the enemy to fire between bullets
    /// </summary>
    public float TimeBetweenBullets = 0.05f;
    /// <summary>
    /// The bullets left for the current status
    /// </summary>
    private int mBulletsLeft;
    /// <summary>
    /// Timer used for rotation movement
    /// </summary>
    private float mTimer;
    /// <summary>
    /// The phase of the attack (used to revert the attack when the boss is in the second phase)
    /// </summary>
    private bool mAttackPhase = false;
    
    public override void OnStateEnter ()
    {
        this.mBulletsLeft = this.NumberOfBullets;
        this.mTimer = this.TimeBetweenBullets;
        this.mAttackPhase = false;
        
        // rotate the boss a bit to the left
        float rotation = this.RotationIncrements * this.NumberOfBullets / 2;
        // rotate halfway through the origin so the center of the attack aligns with the player
        transform.Rotate (Vector3.up, -rotation);
    }

    void FixedUpdate ()
    {
        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;

        this.mTimer = this.TimeBetweenBullets;
        
        Transform current = this.transform;
        // rotate the boss
        transform.Rotate (Vector3.up, this.mAttackPhase == false ? this.RotationIncrements : -this.RotationIncrements);
        // get a bullet from the object pool
        GameObject entry = this.Machine.BulletObjectPool.Pop ();
        
        // set the direction and position
        entry.transform.SetPositionAndRotation (
            current.position,
            current.rotation
        );
        
        // finally activate the attack
        entry.gameObject.SetActive (true);

        this.mBulletsLeft--;

        if (this.mBulletsLeft > 0)
            return;

        if (this.Machine.Controller.Phase == BossPhase.Second && this.mAttackPhase == false)
        {
            transform.Rotate (Vector3.up, this.RotationIncrements / 2);
            
            this.mBulletsLeft = this.NumberOfBullets;
            this.mAttackPhase = true;
            return;
        }
        
        this.Machine.PopState ();
    }
}
