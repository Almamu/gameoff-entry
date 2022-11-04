using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdState : MonoBehaviour
{
    protected BirdStateMachine Machine { get; set; }
    protected BirdMovement MovementState { get; set; }
    protected BirdAttack AttackState { get; set; }
    
    // Start is called before the first frame update
    protected void Awake()
    {
        this.Machine = GetComponent <BirdStateMachine> ();
        this.MovementState = GetComponent <BirdMovement> ();
        this.AttackState = GetComponent <BirdAttack> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnStateEnter()
    {
        
    }

    public virtual void OnStateExit()
    {
        
    }
}
