using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector]
    public PlayerState CurrentState { get; set; }

    private Queue<PlayerState> mStateQueue = new Queue <PlayerState> ();
    
    // Start is called before the first frame update
    void Awake()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent<PlayerState>();
    }

    /// <summary>
    /// Changes the current active state to the newState
    /// </summary>
    /// <param name="newState"></param>
    public void PushState(PlayerState newState)
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
