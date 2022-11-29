using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Enemies.Bosses;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Contains a list of attacks for the boss to use
/// </summary>
public enum BossAttack
{
    ToxicWaste,
    Racimo,
    Wings,
    ToxicWasteFountain,
    Swing,
    MeteorStrike
}

public class BossStateMachine : MonoBehaviour
{
    /// <summary>
    /// Where the attacks should spawn
    /// </summary>
    public Transform AttacksSpawn;
    /// <summary>
    /// The game object to follow while moving
    /// </summary>
    public GameObject Objective;
    /// <summary>
    /// The racimo object pool used to create racimo attacks
    /// </summary>
    public RacimoObjectPool RacimoObjectPool { get; set; }
    /// <summary>
    /// The racimo object pool used to create the second phase attacks
    /// </summary>
    public RacimoSecondPhaseObjectPool RacimoSecondPhaseObjectPool { get; set; }
    /// <summary>
    /// The object pool used for toxic waste attacks
    /// </summary>
    public ToxicWasteObjectPool ToxicWasteObjectPool { get; set; }
    /// <summary>
    /// The object pool used for normal bullets
    /// </summary>
    public ObjectPool BulletObjectPool { get; set; }
    /// <summary>
    /// The current state the machine is in
    /// </summary>
    public BossState CurrentState { get; set; }
    /// <summary>
    /// The controller for the bosses level
    /// </summary>
    public BossesController Controller { get; set; }
    /// <summary>
    /// Animator for the vulture model
    /// </summary>
    public Animator ModelAnimator { get; set; }
    /// <summary>
    /// The particles to play when doing a meteor strike
    /// </summary>
    public ParticleSystem MeteorStrikeParticles { get; set; }
    /// <summary>
    /// The particles to play when doing a wing attack
    /// </summary>
    public ParticleSystem WingsAttackParticles { get; set; }
    /// <summary>
    /// The particle system to use to show wings moving
    /// </summary>
    public ParticleSystem WingsParticles { get; set; }
    /// <summary>
    /// The boolean for the fountain animation
    /// </summary>
    public static readonly int FountainHash = Animator.StringToHash ("Fountain");
    /// <summary>
    /// The wings attack effect
    /// </summary>
    public static readonly int WingsHash = Animator.StringToHash ("Wings");
    /// <summary>
    /// The wings attack effect
    /// </summary>
    public static readonly int StrikeHash = Animator.StringToHash ("Strike");

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
    /// Toxic waste fountain attack state used for transition
    /// </summary>
    private BossToxicWasteFountainAttackState ToxicWasteFountainAttackState;
    /// <summary>
    /// Swing attack state used for transition
    /// </summary>
    private BossSwingAttackState SwingAttackState;
    /// <summary>
    /// Meteor strike attack state used for transition
    /// </summary>
    private BossMeteorStrikeAttackState MeteorStrikeAttackState;
    
    void Awake ()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent <BossState> ();
        this.RacimoObjectPool = GetComponentInParent <RacimoObjectPool> ();
        this.RacimoSecondPhaseObjectPool = GetComponentInParent <RacimoSecondPhaseObjectPool> ();
        this.ToxicWasteObjectPool = GetComponentInParent <ToxicWasteObjectPool> ();
        this.BulletObjectPool = GetComponentInParent <ObjectPool> ();
        this.ToxicWasteAttackState = GetComponent <BossToxicWasteAttackState> ();
        this.RacimoAttackState = GetComponent <BossRacimoAttackState> ();
        this.DashMovementState = GetComponent <BossDashMovementState> ();
        this.WingsAttackState = GetComponent <BossWingsAttack> ();
        this.ToxicWasteFountainAttackState = GetComponent <BossToxicWasteFountainAttackState> ();
        this.SwingAttackState = GetComponent <BossSwingAttackState> ();
        this.MeteorStrikeAttackState = GetComponent <BossMeteorStrikeAttackState> ();

        this.ModelAnimator = this.GetComponentOnlyInChildren <Animator> ();
        this.Controller = GetComponentInParent <BossesController> (true);
        this.MeteorStrikeParticles = this.transform.Find ("GroundSlamRed").GetComponent <ParticleSystem> ();
        this.WingsAttackParticles = this.transform.Find ("DustCircleDark").GetComponent <ParticleSystem> ();
        this.WingsParticles = this.transform.Find ("WingsParticles").GetComponent <ParticleSystem> ();
        
        // subscribe to required events to alter state
        CombatEventManager.DisableMovement += OnDisableMovement;
        CombatEventManager.EnableMovement += OnEnableMovement;
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

    public void SwitchToAttack (BossAttack attack)
    {
        // ensure the player is in the movement state
        if (this.CurrentState is not BossMovementState)
            return;
        
        BossState state = attack switch
        {
            BossAttack.Racimo => this.RacimoAttackState,
            BossAttack.ToxicWaste => this.ToxicWasteAttackState,
            BossAttack.Wings => this.WingsAttackState,
            BossAttack.ToxicWasteFountain => this.ToxicWasteFountainAttackState,
            BossAttack.Swing => this.SwingAttackState,
            BossAttack.MeteorStrike => this.MeteorStrikeAttackState,
            _ => throw new InvalidDataException ("Trying to switch to an unknown attack")
        };
        
        // get into the requested state
        this.PushState (state);
    }
    
    private void OnCollisionEnter (Collision collision)
    {
        if (this.enabled == false)
            return;

        if (collision.gameObject.CompareTag ("Player Bullet") == true)
            this.Controller.SendMessage ("ApplyDamage");
        if (collision.gameObject.CompareTag ("Player") == true)
            collision.gameObject.SendMessage ("ApplyBossDamage", transform.position);
    }
}
