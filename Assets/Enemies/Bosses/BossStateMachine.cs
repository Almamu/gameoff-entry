using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.Bosses;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossStateMachine : MonoBehaviour
{
    /// <summary>
    /// The game object to follow while moving
    /// </summary>
    public GameObject Objective;
    /// <summary>
    /// The racimo object pool used to create racimo attacks
    /// </summary>
    public RacimoObjectPool RacimoObjectPool { get; set; }
    /// <summary>
    /// The object pool used for toxic waste attacks
    /// </summary>
    public ToxicWasteObjectPool ToxicWasteObjectPool { get; set; }
    /// <summary>
    /// The probability of a toxic waste attack
    /// </summary>
    public float ToxicWasteAttackProbability = 0.4f;
    /// <summary>
    /// The probability of a racimo attack
    /// </summary>
    public float RacimoAttackProbability = 0.1f;
    /// <summary>
    /// The probability of a dash
    /// </summary>
    public float DashMovementProbability = 0.6f;
    /// <summary>
    /// The current state the machine is in
    /// </summary>
    public BossState CurrentState { get; set; }
    /// <summary>
    /// The controller for the bosses level
    /// </summary>
    public BossesController Controller { get; set; }

    /// <summary>
    /// The list of states to unwind when poping them
    /// </summary>
    private Queue<BossState> mStateQueue = new Queue <BossState> ();

    /// <summary>
    /// Toxic waste attack state used for transitions
    /// </summary>
    private BossToxicWasteAttackState ToxicWasteAttackState;
    /// <summary>
    /// Racimo attack state used for transition
    /// </summary>
    private BossRacimoAttackState RacimoAttackState;
    /// <summary>
    /// Dash movement state used for transition
    /// </summary>
    private BossDashMovementState DashMovementState;
    /// <summary>
    /// Wings attack state used for transition
    /// </summary>
    private BossWingsAttack WingsAttackState;
    /// <summary>
    /// Toxic waste fountain attack
    /// </summary>
    private BossToxicWasteFountainAttackState ToxicWasteFountainAttackState;
    
    void Awake ()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent <BossState> ();
        this.RacimoObjectPool = GetComponentInParent <RacimoObjectPool> ();
        this.ToxicWasteObjectPool = GetComponentInParent <ToxicWasteObjectPool> ();
        this.ToxicWasteAttackState = GetComponent <BossToxicWasteAttackState> ();
        this.RacimoAttackState = GetComponent <BossRacimoAttackState> ();
        this.DashMovementState = GetComponent <BossDashMovementState> ();
        this.WingsAttackState = GetComponent <BossWingsAttack> ();
        this.ToxicWasteFountainAttackState = GetComponent <BossToxicWasteFountainAttackState> ();
        
        this.Controller = GetComponentInParent <BossesController> (true);
        
        // subscribe to required events to alter state
        EventManager.DisableMovement += OnDisableMovement;
        EventManager.EnableMovement += OnEnableMovement;
    }

    /// <summary>
    /// Changes the current active state to the newState
    /// </summary>
    /// <param name="newState"></param>
    public void PushState (BossState newState)
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
    
    void OnEnableMovement ()
    {
        // enable current state
        this.enabled = true;
        this.CurrentState.enabled = true;
    }

    void OnDisableMovement ()
    {
        // disable current state
        this.enabled = false;
        this.CurrentState.enabled = false;
    }

    public void RandomNextState ()
    {
        float random = Random.Range (0.0f, 1.0f);

        // decide the next state based on the probability values
        // or do nothing
        if (random <= this.RacimoAttackProbability)
            this.PushState (this.RacimoAttackState);
        else if (random <= this.ToxicWasteAttackProbability)
            this.PushState (this.ToxicWasteAttackState);
        else if (random <= this.DashMovementProbability)
            this.PushState (this.DashMovementState);
    }
}
