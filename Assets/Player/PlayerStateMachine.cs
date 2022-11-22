using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    /// <summary>
    /// The current state the machine is in
    /// </summary>
    public PlayerState CurrentState { get; set; }
    /// <summary>
    /// The animator used by the player
    /// </summary>
    public Animator Animator { get; set; }
    /// <summary>
    /// The time the invulnerability timer is effective for
    /// </summary>
    public float InvulnerabilityTimer = 3.5f;
    /// <summary>
    /// The amount of force applied against the player when receiving certain types of damage
    /// </summary>
    public float DamagePushbackForce = 8.0f;
    /// <summary>
    /// Event fired when the health value is changed
    /// </summary>
    public static event Action <float> HealthUpdate;

    private Queue<PlayerState> mStateQueue = new Queue <PlayerState> ();

    /// <summary>
    /// The health left for the player
    /// </summary>
    private float mHealth = 1.0f;
    /// <summary>
    /// Timer for invulnerability
    /// </summary>
    private float mInvulnerabilityTimer = 0.0f;
    /// <summary>
    /// The rigidbody the player is using
    /// </summary>
    private Rigidbody mRigidbody;
    /// <summary>
    /// Velocity of the rigidbody before being disabled
    /// </summary>
    private Vector3 mSavedVelocity;

    /// <summary>
    /// The ID of the InvulnerabilityTimer in the animator
    /// </summary>
    private static readonly int InvulnerabilityTimerId = Animator.StringToHash ("InvulnerabilityTimer");
    /// <summary>
    /// The collision layer at which the birds are at
    /// </summary>
    public static int BirdsCollisionLayer;
    /// <summary>
    /// The collision layer at which the birds are at
    /// </summary>
    public static int PlayerCollisionLayer;
    /// <summary>
    /// The collision layer at which the flying attacks are at
    /// </summary>
    public static int FlyingAttacksLayer;
    /// <summary>
    /// The collision layer at which the boss enemies are at
    /// </summary>
    public static int BossEnemyLayer;

    /// <summary>
    /// The bounce state used when receiving damage that moves the player
    /// </summary>
    private PlayerBounceState BounceState;
    
    void Awake()
    {
        BirdsCollisionLayer = LayerMask.NameToLayer ("Enemies");
        PlayerCollisionLayer = LayerMask.NameToLayer ("Player");
        FlyingAttacksLayer = LayerMask.NameToLayer ("Flying attacks");
        BossEnemyLayer = LayerMask.NameToLayer ("Boss enemy");
        
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent<PlayerState>();
        // get reference to the animator
        this.Animator = GetComponent <Animator> ();
        // get the rigidbody
        this.mRigidbody = GetComponent <Rigidbody> ();
        // bounce state for receiving damage that moves the player
        this.BounceState = GetComponent <PlayerBounceState> ();
        
        // subscribe to required events to alter state
        EventManager.DisableMovement += OnDisableMovement;
        EventManager.EnableMovement += OnEnableMovement;
    }

    /// <summary>
    /// Changes the current active state to the newState
    /// </summary>
    /// <param name="newState"></param>
    public void PushState (PlayerState newState)
    {
        this.mStateQueue.Enqueue(this.CurrentState);
        
        // disable current state
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        
        // enable new state and set it as current
        this.CurrentState = newState;
        this.CurrentState.enabled = this.enabled;
        this.CurrentState.OnStateEnter();
    }

    /// <summary>
    /// Deactivates the current state and enables the last one in the queue
    /// </summary>
    public void PopState ()
    {
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        this.CurrentState = this.mStateQueue.Dequeue();
        this.CurrentState.enabled = this.enabled;
        this.CurrentState.OnStateEnter();
    }

    public bool IsInvulnerable ()
    {
        return this.mInvulnerabilityTimer > 0.0f;
    }

    void FixedUpdate ()
    {
        Debug.Log (this.mRigidbody.velocity);
        
        // set the invulnerability timer so the animation plays
        this.Animator.SetFloat (InvulnerabilityTimerId, this.mInvulnerabilityTimer);
        
        if (this.mInvulnerabilityTimer <= 0.0f)
            return;

        this.mInvulnerabilityTimer -= Time.fixedDeltaTime;

        if (this.mInvulnerabilityTimer > 0.0f || this.CurrentState is PlayerDodge)
            return;

        // set collision layers so birds can hit the player again
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, BirdsCollisionLayer, false);
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, FlyingAttacksLayer, false);
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, BossEnemyLayer, false);
    }
    
    void OnEnableMovement ()
    {
        // enable current state
        this.enabled = true;
        this.CurrentState.enabled = true;
        this.mRigidbody.isKinematic = false;
        this.mRigidbody.detectCollisions = true;
        this.mRigidbody.velocity = this.mSavedVelocity;
    }

    void OnDisableMovement ()
    {
        // disable current state
        this.enabled = false;
        this.CurrentState.enabled = false;
        this.mSavedVelocity = this.mRigidbody.velocity;
        this.mRigidbody.isKinematic = true;
        this.mRigidbody.detectCollisions = false;
    }

    public void ApplyDamage (float amount)
    {
        // invulnerability timer means no damage!
        if (this.IsInvulnerable () == true)
            return;
        
        // start invulnerability timer
        this.mInvulnerabilityTimer = this.InvulnerabilityTimer;
        // decrease lifes
        this.mHealth -= amount;
        // update the health
        HealthUpdate?.Invoke (this.mHealth);
        // set collision layers so birds cannot hit the player
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, BirdsCollisionLayer, true);
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, FlyingAttacksLayer, true);
        Physics.IgnoreLayerCollision (PlayerCollisionLayer, BossEnemyLayer, true);
    }

    public void ApplyHitDamage ()
    {
        this.ApplyDamage (0.1f);
    }

    public void ApplyToxicDamage ()
    {
        this.ApplyDamage (0.1f);
    }
    
    public void ApplyWindDamage ()
    {
        this.ApplyDamage (0.1f);
    }

    public void ApplyBossDamage (Vector3 center)
    {
        this.ApplyDamage (0.1f);

        // enter the bounce state
        this.PushState (this.BounceState);
        
        // pushback the player with force
        Vector3 direction = (transform.position - center).normalized;

        // ensure the direction points somewhere
        if (math.abs (direction.x) < 0.1f || math.abs (direction.z) < 0.1f)
            direction.x = 1.0f;

        // go up a little bit so the force applied actually moves the player
        direction.y = 0.1f;

        // apply the force
        this.mRigidbody.velocity = direction * this.DamagePushbackForce;
    }
}
