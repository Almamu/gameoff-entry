using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class BossToxicWasteFountainAttackState : BossState
{
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
    }

    private void ShootFor (Vector3 target)
    {
        GameObject obj = this.Machine.ToxicWasteObjectPool.Pop ();
        
        // setup the attack
        ToxicAttackBehaviour behaviour = obj.GetComponent <ToxicAttackBehaviour> ();
        behaviour.Target = target;
        behaviour.transform.position = behaviour.StartPosition = this.transform.position;
        
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