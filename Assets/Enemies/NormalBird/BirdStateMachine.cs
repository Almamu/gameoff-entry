using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BirdStateMachine : MonoBehaviour
{
    public BirdState CurrentState { get; private set; }
    
    public Vector3 SpawnerCenter { get; private set; }

    public BoxCollider MovementArea { get; private set; }
    
    public GameObject Player { get; private set; }
    
    private Queue<BirdState> mStateQueue;

    void Start()
    {
        // get the current, active state so the state machine has something to do
        this.CurrentState = GetComponent<BirdState>();
        // clear the state list
        this.mStateQueue = new Queue <BirdState> ();
        // get the movement area
        this.MovementArea = this.transform.parent.Find ("MovementArea").GetComponent <BoxCollider> ();
        // get the spawn's center
        this.SpawnerCenter = this.MovementArea.transform.position;
        // find the player
        this.Player = GameObject.FindWithTag ("Player");
        // fire the stateenter event
        this.CurrentState.OnStateEnter ();
        
        // subscribe to required events to alter state
        EventManager.DisableMovement += OnDisableMovement;
        EventManager.EnableMovement += OnEnableMovement;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag ("Player Bullet") == false)
            return;
        
        // send message for death to the parent
        this.transform.parent.SendMessage ("OnEnemyDead", this.gameObject);
        // finally deactivate ourselves
        this.gameObject.SetActive (false);
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
    
    void OnEnableMovement ()
    {
        // enable current state
        this.CurrentState.enabled = true;
    }

    void OnDisableMovement ()
    {
        // disable current state
        this.CurrentState.enabled = false;
    }
}
