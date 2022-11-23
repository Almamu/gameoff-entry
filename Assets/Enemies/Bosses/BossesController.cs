using System;
using System.IO;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossesController : MonoBehaviour
{
    enum BossPhase
    {
        Starting = 0,
        First = 1,
        FirstToSecond = 2,
        Second = 3,
        Finished = 4
    }
    
    /// <summary>
    /// The total health of the bosses
    /// </summary>
    public float Health = 100.0f;
    /// <summary>
    /// The minimum health to pass onto the second phase
    /// </summary>
    public float SecondPhaseHealthThreshold = 50.0f;

    /// <summary>
    /// Minimum time for a state change
    /// </summary>
    public float MinStateChangeTime = 2.0f;
    /// <summary>
    /// Maximum time for a state change
    /// </summary>
    public float MaxStateChangeTime = 4.0f;
    /// <summary>
    /// The playable area for the bosses
    /// </summary>
    public BoxCollider PlayableArea { get; set; }
    /// <summary>
    /// State machines of all the bosses created
    /// </summary>
    private BossStateMachine[] mBosses;

    /// <summary>
    /// Timer until next state change
    /// </summary>
    private float mStateChangeTimer = 0.0f;
    /// <summary>
    /// The current health the boss has
    /// </summary>
    private float mHealth = 0.0f;
    /// <summary>
    /// Indicates the current state of the boss
    /// </summary>
    private BossPhase mPhase = BossPhase.Starting;
    /// <summary>
    /// The function to handle the current phase
    /// </summary>
    private Action mPhaseHandler;
    
    void Start ()
    {
        this.ResetChangeTimer ();
        
        // get access to both state machines as these will be directed from here
        this.mBosses = GetComponentsInChildren <BossStateMachine> ();
        this.mHealth = this.Health;
        this.PlayableArea = transform.Find ("PlayableArea").GetComponent <BoxCollider> ();
        
        // validate found bosses as this script only works with a specific amount of bosses
        if (this.mBosses.Length != 2)
            throw new InvalidDataException ("This controller only works with two bosses");
        
        // get into the first phase
        this.TransitionToPhase (BossPhase.Starting);
    }

    private void ResetChangeTimer ()
    {
        this.mStateChangeTimer = Random.Range (this.MinStateChangeTime, this.MaxStateChangeTime);
    }

    private bool IsTimerExpired ()
    {
        return !(this.mStateChangeTimer > 0.0f);
    }
    
    void FixedUpdate ()
    {
        this.mStateChangeTimer -= Time.fixedDeltaTime;
        // handle whatever the phase has to do
        this.mPhaseHandler ();
        
        if (this.IsTimerExpired () == false)
            return;

        this.ResetChangeTimer ();
    }


    void TransitionToPhase (BossPhase phase)
    {
        this.mPhaseHandler = phase switch
        {
            BossPhase.Starting      => this.HandleStartingPhase,
            BossPhase.First         => this.HandleFirstPhase,
            BossPhase.FirstToSecond => this.HandleFirstToSecondPhase,
            BossPhase.Second        => this.HandleSecondPhase,
            BossPhase.Finished      => this.HandleFinishPhase,
            _                       => throw new InvalidDataException ("Trying to transition to an unknown boss phase")
        };

        this.mPhase = phase;
    }

    void HandleStartingPhase ()
    {
        // TODO: IMPLEMENT ANIMATIONS
        this.TransitionToPhase (BossPhase.First);
    }

    void HandleFirstPhase ()
    {
        // check left life and switch to second phase if required
        if (this.mHealth <= this.SecondPhaseHealthThreshold)
        {
            this.TransitionToPhase (BossPhase.FirstToSecond);
            return;
        }

        if (this.IsTimerExpired () == false)
            return;
        
        // decide on next attack
        BossAttack firstAttack = EnumExtensions.RandomArray (
            BossAttack.Racimo,
            BossAttack.Swing,
            BossAttack.ToxicWaste
        );
        BossAttack secondAttack = firstAttack == BossAttack.ToxicWaste
            ? EnumExtensions.RandomArray (BossAttack.Racimo, BossAttack.Swing)
            : EnumExtensions.RandomArray (
                BossAttack.Racimo,
                BossAttack.Swing,
                BossAttack.ToxicWaste
            );
        
        // toxic waste cannot be thrown by both bosses at the same time
        this.mBosses[0].SwitchToAttack (firstAttack);
        this.mBosses[1].SwitchToAttack (secondAttack);
    }

    void HandleFirstToSecondPhase ()
    {
        // disable one of the bosses and switch to the second phase
        this.mBosses[1].gameObject.SetActive (false);
        this.TransitionToPhase (BossPhase.Second);
    }

    void HandleSecondPhase ()
    {
        // TODO:
        // second phase
        // racimo double explosion
        // toxic waste bigger and longer
        // swing faster and twice
        // toxic fountain, as big and long as phase 1
        // meteor strike when too far away
        // if you get too close the boss should push you with wind, not periodic
    }

    void HandleFinishPhase ()
    {
        
    }

    /// <summary>
    /// Message used to apply some damage from the player
    /// </summary>
    void ApplyDamage ()
    {
        // TODO: EXPOSE THIS?
        this.mHealth -= 1.0f;

        EventManager.InvokeBossHealth (this.mHealth);
    }
}
