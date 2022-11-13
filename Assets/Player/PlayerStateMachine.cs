using System;
using System.Collections.Generic;
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
    /// The number of lifes the player starts the level
    /// </summary>
    public int StartingLifes = 5;
    /// <summary>
    /// The time the invulnerability timer is effective for
    /// </summary>
    public float InvulnerabilityTimer = 3.5f;

    /// <summary>
    /// Event fired when the player loses a life
    /// </summary>
    public static event Action <int> LifeLost;
    /// <summary>
    /// Event fired when the player picks up a life
    /// </summary>
    public static event Action <int> LifeFound;

    private Queue<PlayerState> mStateQueue = new Queue <PlayerState> ();

    private int mLifesLeft = 0;
    private float mInvulnerabilityTimer = 0.0f;

    /// <summary>
    /// The mesh renderer for this object
    /// </summary>
    private MeshRenderer mRenderer;

    /// <summary>
    /// The ID of the InvulnerabilityTimer in the animator
    /// </summary>
    private static readonly int InvulnerabilityTimerId = Animator.StringToHash ("InvulnerabilityTimer");

    // Start is called before the first frame update
    void Awake()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent<PlayerState>();
        // get reference to the animator
        this.Animator = GetComponent <Animator> ();
        
        // subscribe to required events to alter state
        EventManager.DisableMovement += OnDisableMovement;
        EventManager.EnableMovement += OnEnableMovement;
    }

    /// <summary>
    /// Changes the current active state to the newState
    /// </summary>
    /// <param name="newState"></param>
    public void PushState(PlayerState newState)
    {
        this.mStateQueue.Enqueue(this.CurrentState);
        
        // disable current state
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        
        // enable new state and set it as current
        this.CurrentState = newState;
        this.CurrentState.enabled = true;
        this.CurrentState.OnStateEnter();
    }

    /// <summary>
    /// Deactivates the current state and enables the last one in the queue
    /// </summary>
    public void PopState()
    {
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        this.CurrentState = this.mStateQueue.Dequeue();
        this.CurrentState.enabled = true;
        this.CurrentState.OnStateEnter();
    }

    public bool IsInvulnerable ()
    {
        return this.mInvulnerabilityTimer > 0.0f;
    }

    void FixedUpdate ()
    {
        // set the invulnerability timer so the animation plays
        this.Animator.SetFloat (InvulnerabilityTimerId, this.mInvulnerabilityTimer);
        
        if (this.mInvulnerabilityTimer <= 0.0f)
            return;

        this.mInvulnerabilityTimer -= Time.fixedDeltaTime;
    }
    
    void OnEnableMovement()
    {
        // enable current state
        this.CurrentState.enabled = true;
    }

    void OnDisableMovement()
    {
        // disable current state
        this.CurrentState.enabled = false;
    }

    public void DecreaseLife ()
    {
        LifeLost?.Invoke (--this.mLifesLeft);
    }

    public void IncreaseLife ()
    {
        LifeFound?.Invoke (++this.mLifesLeft);
    }

    public void ApplyDamage ()
    {
        // start invulnerability timer
        this.mInvulnerabilityTimer = this.InvulnerabilityTimer;
        // decrease lifes
        this.DecreaseLife ();
        // TODO: ANIMATE THE PLAYER
    }
}
