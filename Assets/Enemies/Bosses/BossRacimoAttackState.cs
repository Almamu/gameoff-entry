using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRacimoAttackState : BossState
{
    public override void OnStateEnter ()
    {
        // get one from the pool
        GameObject entry = this.Machine.RacimoObjectPool.Pop ();
        
        // set the shots so they go in the direction the boss is looking at
        entry.transform.rotation = this.transform.rotation;
        entry.transform.position = this.transform.position;
        
        // finally activate the attack
        entry.gameObject.SetActive (true);
        
        // exit this state
        this.Machine.PopState ();
    }
}
