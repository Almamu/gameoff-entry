using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossToxicWasteAttackState : BossState
{
    public override void OnStateEnter ()
    {
        GameObject obj = this.Machine.ToxicWasteObjectPool.Pop ();
        
        // setup the attack
        ToxicAttackBehaviour behaviour = obj.GetComponent <ToxicAttackBehaviour> ();
        behaviour.Target = this.Machine.Objective.transform.position;
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
        // immediately exit the state
        this.Machine.PopState ();
    }
}
