using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class BossToxicWasteFountainAttackState : BossState
{
    /// <summary>
    /// The scale for the toxic waste
    /// </summary>
    public float WasteScale = 0.4f;
    /// <summary>
    /// The radius for the hitbox of the waste
    /// </summary>
    public float WasteRadius = 2.0f;
    /// <summary>
    /// The time it takes for the wastes to disappear
    /// </summary>
    public float DisappearanceTimer = 30.0f;
    /// <summary>
    /// Time to wait between cycles
    /// </summary>
    public float CycleTimer = 0.3f;
    /// <summary>
    /// The amount of shoots done per cycle
    /// </summary>
    public int QuantityPerCycle = 3;
    /// <summary>
    /// The total amount of toxic waste to shoot
    /// </summary>
    public int TotalQuantity = 15;

    private float mTimer = 0.0f;

    private int mQuantityLeft = 0;
    
    public override void OnStateEnter ()
    {
        this.mTimer = this.CycleTimer;
        this.mQuantityLeft = this.TotalQuantity;

        this.Machine.ModelAnimator.SetBool (BossStateMachine.FountainHash, true);
    }

    public override void OnStateExit ()
    {
        this.Machine.ModelAnimator.SetBool (BossStateMachine.FountainHash, false);
    }

    private void ShootFor (Vector3 target)
    {
        GameObject obj = this.Machine.ToxicWasteObjectPool.Pop ();
        
        // setup the attack
        ToxicAttackBehaviour behaviour = obj.GetComponent <ToxicAttackBehaviour> ();
        behaviour.Target = target;
        behaviour.transform.position = behaviour.StartPosition = this.Machine.AttacksSpawn.position;
        behaviour.DisappearanceTimer = this.DisappearanceTimer;
        behaviour.Scale = this.WasteScale;
        behaviour.Radius = this.WasteRadius;
        
        // calculate speed difference
        Vector3 distance = behaviour.StartPosition - behaviour.Target;

        // set speed a bit higher if the attack is too far away
        if (distance.sqrMagnitude > 400.0f)
            behaviour.FinalSpeed = behaviour.Speed + 0.2f;
        else
            behaviour.FinalSpeed = behaviour.Speed;
        
        // enable the attack
        obj.gameObject.SetActive (true);
    }

    void FixedUpdate ()
    {
        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;

        for (int i = 0; i < this.QuantityPerCycle && this.mQuantityLeft > 0; i++)
        {
            // get a random position from the movement area
            Vector3 target = this.Machine.Controller.PlayableArea.bounds.GetRandomPositionInside ();
            // shoot to a random position
            this.ShootFor (target);
            // decrease the counter
            this.mQuantityLeft--;
        }
        
        // reset the wait timer
        this.mTimer = this.CycleTimer;
        // if nothing left to throw, pop the state
        if (this.mQuantityLeft <= 0)
            this.Machine.PopState ();
    }
}
