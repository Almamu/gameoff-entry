using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdQuickAttack : BirdState
{
    /// <summary>
    /// Starting speed
    /// </summary>
    public float StartSpeed = 25.0f;
    /// <summary>
    /// Speed halfway through
    /// </summary>
    public float EndSpeed = 30.0f;
    /// <summary>
    /// The speed to target when slowing down
    /// </summary>
    public float SlowdownToSpeed = 5.0f;
    /// <summary>
    /// The distance to slowdown the attack
    /// </summary>
    public float SlowdownDistance = 5.0f;
    /// <summary>
    /// The current speed the bird is moving at
    /// </summary>
    private float mCurrentSpeed = 0.0f;

    private Vector3 mDestination;
    private Vector3 mDirection;

    private bool mHasAttacked = false;
    
    public override void OnStateEnter ()
    {
        // set the starting speed
        this.mCurrentSpeed = this.StartSpeed;
        // set the destination
        this.mDestination = this.Machine.Player.transform.position;
        this.mDirection = (this.mDestination - this.transform.position).normalized;
        this.mHasAttacked = false;
        
        // play the attack sound
        this.Machine.AudioSource.PlayOneShot (this.Machine.AttackAudio);

        // look at the destination
        transform.rotation = Quaternion.LookRotation (this.mDirection);
    }

    bool MoveTowardsDestination ()
    {
        float speedDelta = Time.fixedDeltaTime * this.mCurrentSpeed;

        transform.position = Vector3.MoveTowards (transform.position, this.mDestination, speedDelta);
        
        // check how far we're
        return Vector3.Distance (transform.position, this.mDestination) <= speedDelta;
    }
    
    void HandleAttack ()
    {
        if (this.mHasAttacked == true)
            return;

        // lerp speed
        this.mCurrentSpeed = Mathf.Lerp (this.mCurrentSpeed, this.EndSpeed, 0.4f);
        
        if (this.MoveTowardsDestination () == false)
            return;

        // set the new destination
        this.mDestination += this.mDirection * this.SlowdownDistance;
        // stop attack mode
        this.mHasAttacked = true;
    }

    void HandleSlowdown ()
    {
        if (this.mHasAttacked == false)
            return;

        // lerp speed
        this.mCurrentSpeed = Mathf.Lerp (this.mCurrentSpeed, this.SlowdownToSpeed, 0.1f);
        
        if (this.MoveTowardsDestination () == false)
            return;
        
        // finished, pop state
        this.Machine.PopState ();
    }
    
    void FixedUpdate ()
    {
        this.HandleAttack ();
        this.HandleSlowdown ();
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (this.enabled == false || collision.gameObject.CompareTag ("Player") == false)
            return;

        collision.gameObject.SendMessage ("ApplyHitDamage");
    }

    private void OnDrawGizmos ()
    {
        // do not draw gizmos if disabled
        if (this.enabled == false)
            return;
        
        Vector3 position = transform.position;

        Color color = Color.green;

        if (this.mHasAttacked == true)
            color = Color.red;
        
        // draw a ball at the destination point
        Gizmos.color = color;
        Gizmos.DrawSphere (this.mDestination, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine (position, this.mDestination);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine (position, transform.forward);
    }
}
