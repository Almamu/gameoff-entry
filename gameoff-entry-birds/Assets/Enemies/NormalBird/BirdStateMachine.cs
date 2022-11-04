using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdStateMachine : MonoBehaviour
{
    [HideInInspector]
    public BirdState CurrentState { get; set; }

    private Queue<BirdState> mStateQueue = new Queue <BirdState> ();
    
    // Start is called before the first frame update
    void Awake()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent<BirdState>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player Bullet") == true)
            Destroy(gameObject);
    }

    /// <summary>
    /// Changes the current active state to the newState
    /// </summary>
    /// <param name="newState"></param>
    public void PushState(BirdState newState)
    {
        this.mStateQueue.Enqueue(this.CurrentState);
        
        // disable current state
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        
        // enable new state and set it as current
        this.CurrentState = newState;
        this.CurrentState.enabled = true;
        this.CurrentState.OnStateEnter();
    }

    /// <summary>
    /// Deactivates the current state and enables the last one in the queue
    /// </summary>
    public void PopState()
    {
        this.CurrentState.OnStateExit();
        this.CurrentState.enabled = false;
        this.CurrentState = this.mStateQueue.Dequeue();
        this.CurrentState.enabled = true;
        this.CurrentState.OnStateEnter();
    }
}
