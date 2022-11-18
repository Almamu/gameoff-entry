using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdState : MonoBehaviour
{
    protected BirdStateMachine Machine { get; set; }
    protected BirdMovement MovementState { get; set; }
    protected BirdState AttackState { get; set; }
    
    // Start is called before the first frame update
    void Awake()
    {
        this.Machine = GetComponent <BirdStateMachine> ();
        this.MovementState = GetComponent <BirdMovement> ();
        this.AttackState = GetComponent <BirdAttack> () ?? (BirdState) GetComponent <BirdQuickAttack>();
    }

    public virtual void OnStateEnter()
    {
        
    }

    public virtual void OnStateExit()
    {
        
    }
}
