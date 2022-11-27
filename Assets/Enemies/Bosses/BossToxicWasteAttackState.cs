using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BossToxicWasteAttackState : BossState
{
    /// <summary>
    /// The time it takes for the waste to disappear on the first phase
    /// </summary>
    public float FirstPhaseWasteTime = 10.0f;
    /// <summary>
    /// The time it takes for the waste to disappear on the second phase
    /// </summary>
    public float SecondPhaseWasteTime = 30.0f;
    /// <summary>
    /// The size of the waste when spawned in the first phase
    /// </summary>
    public float FirstPhaseWasteScale = 0.4f;
    /// <summary>
    /// The size of the waste when spawned in the second phase
    /// </summary>
    public float SecondPhaseWasteScale = 0.6f;
    /// <summary>
    /// The radius for the hitbox in the first phase
    /// </summary>
    public float FirstPhaseRadius = 2.0f;
    /// <summary>
    /// The radius for the hitbox in the second phase
    /// </summary>
    public float SecondPhaseRadius = 3.0f;

    public override void OnStateEnter ()
    {
        GameObject obj = this.Machine.ToxicWasteObjectPool.Pop ();
        
        // setup the attack
        ToxicAttackBehaviour behaviour = obj.GetComponent <ToxicAttackBehaviour> ();
        behaviour.Target = this.Machine.Objective.transform.position;
        behaviour.transform.position = behaviour.StartPosition = this.Machine.AttacksSpawn.position;
        
        behaviour.Radius = this.Machine.Controller.Phase switch
        {
            BossPhase.First  => this.FirstPhaseRadius,
            BossPhase.Second => this.SecondPhaseRadius,
            _                => throw new InvalidDataException ("This should not happen")
        };
        
        behaviour.Scale = this.Machine.Controller.Phase switch
        {
            BossPhase.First  => this.FirstPhaseWasteScale,
            BossPhase.Second => this.SecondPhaseWasteScale,
            _                => throw new InvalidDataException("This should not happen")
        };

        behaviour.DisappearanceTimer = this.Machine.Controller.Phase switch
        {
            BossPhase.First  => this.FirstPhaseWasteTime,
            BossPhase.Second => this.SecondPhaseWasteTime,
            _                => throw new InvalidDataException ("This should not happen")
        };
        

        // calculate speed difference
        Vector3 distance = behaviour.StartPosition - behaviour.Target;

        // set speed a bit higher if the attack is too far away
        if (distance.sqrMagnitude > 400.0f)
            behaviour.FinalSpeed = behaviour.Speed + 0.2f;
        else
            behaviour.FinalSpeed = behaviour.Speed;
        
        // enable the attack
        obj.gameObject.SetActive (true);
        // immediately exit the state
        this.Machine.PopState ();
    }
}
