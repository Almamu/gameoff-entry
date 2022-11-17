using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    protected BossStateMachine Machine { get; set; }
    protected BossToxicWasteAttackState ToxicWasteAttack { get; set; }
    
    protected void Awake ()
    {
        this.Machine = GetComponent <BossStateMachine> ();
        this.ToxicWasteAttack = this.GetComponent <BossToxicWasteAttackState> ();
    }
    
    public virtual void OnStateEnter ()
    {
        
    }

    public virtual void OnStateExit ()
    {
        
    }
}
