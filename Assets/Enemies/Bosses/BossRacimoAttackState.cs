using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BossRacimoAttackState : BossState
{
    public override void OnStateEnter ()
    {
        Transform current = this.transform;
        // get one from the pool
        GameObject entry = this.Machine.Controller.Phase switch
        {
            BossPhase.First  => this.Machine.RacimoObjectPool.Pop (),
            BossPhase.Second => this.Machine.RacimoSecondPhaseObjectPool.Pop (),
            _                => throw new InvalidDataException ("This should not happen")
        };

        // set the shots so they go in the direction the boss is looking at
        entry.transform.SetPositionAndRotation (
            current.position,
            current.rotation
        );

        // finally activate the attack
        entry.gameObject.SetActive (true);
        
        // exit this state
        this.Machine.PopState ();
    }
}
