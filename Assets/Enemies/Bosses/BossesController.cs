using System;
using System.IO;
using Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum BossPhase
{
    Starting = 0,
    First = 1,
    FirstToSecond = 2,
    Second = 3,
    Finished = 4
}

public class BossesController : MonoBehaviour
{
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
    /// The minimum distance for a meteor strike to happen on the second phase
    /// </summary>
    public float DistanceForMeteorStrike = 1000.0f;
    /// <summary>
    /// The maximum distance for the wings attack to happen on the second phase
    /// </summary>
    public float MaximumDistanceForWings = 5.0f;
    /// <summary>
    /// The cooldown for the fountain attack
    /// </summary>
    public float FountainCooldown = 40.0f;
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
    public BossPhase Phase { get; set; } = BossPhase.Starting;
    /// <summary>
    /// The function to handle the current phase
    /// </summary>
    private Action mPhaseHandler;
    /// <summary>
    /// The cooldown timer for the fountain attack
    /// </summary>
    private float mFountainCooldownTimer = 0.0f;
    
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

        this.Phase = phase;

        if (this.Phase == BossPhase.Starting)
        {
            CombatEventManager.InvokeTextbox ("COMBAT.BOSS_START");
            CombatEventManager.EnableMovement += this.HandleTransition;
        }
        else if (this.Phase == BossPhase.FirstToSecond)
        {
            CombatEventManager.InvokeTextbox ("COMBAT.SECOND_PHASE");
            CombatEventManager.EnableMovement += this.HandleSecondPhaseTransition;
        }
    }

    void HandleStartingPhase ()
    {
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
    }

    void HandleTransition ()
    {
        CombatEventManager.EnableMovement -= this.HandleTransition;
        
        // transition to the real first phase
        this.TransitionToPhase (BossPhase.First);
    }

    void HandleSecondPhaseTransition ()
    {
        CombatEventManager.EnableMovement -= this.HandleSecondPhaseTransition;
        
        // transition to the real second phase
        this.TransitionToPhase (BossPhase.Second);
    }

    void HandleSecondPhase ()
    {
        // decrement the fountain cooldown timer
        if (this.mFountainCooldownTimer >= 0.0f)
            this.mFountainCooldownTimer -= Time.fixedDeltaTime;

        if (this.mHealth <= 0.0f)
        {
            this.TransitionToPhase (BossPhase.Finished);
            return;
        }

        if (this.IsTimerExpired () == false)
            return;
        
        // check if the player is too far away and do a meteor strike
        Vector3 distance = this.mBosses [0].transform.position - this.mBosses [0].Objective.transform.position;
        
        BossAttack attack;
        
        if (distance.magnitude > this.DistanceForMeteorStrike)
        {
            attack = BossAttack.MeteorStrike;
        }
        else if (distance.magnitude < this.MaximumDistanceForWings)
        {
            attack = BossAttack.Wings;
        }
        else if (this.mHealth < (this.SecondPhaseHealthThreshold / 2) && this.mFountainCooldownTimer < 0.0f)
        {
            attack = EnumExtensions.RandomIgnore (BossAttack.MeteorStrike, BossAttack.Wings);
        }
        else
        {
            attack = EnumExtensions.RandomIgnore (BossAttack.MeteorStrike, BossAttack.ToxicWasteFountain, BossAttack.Wings);
        }

        this.mBosses [0].SwitchToAttack (attack);

        if (attack == BossAttack.ToxicWasteFountain)
            this.mFountainCooldownTimer = this.FountainCooldown;
    }

    void HandleFinishPhase ()
    {
        CombatEventManager.ClearEvents ();
        SceneManager.LoadScene ("Credits");
    }

    /// <summary>
    /// Message used to apply some damage from the player
    /// </summary>
    void ApplyDamage ()
    {
        // TODO: EXPOSE THIS?
        this.mHealth -= 1.0f;

        CombatEventManager.InvokeBossHealth (this.mHealth);
    }
}
