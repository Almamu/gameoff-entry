using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossesController : MonoBehaviour
{
    /// <summary>
    /// The total health of the bosses
    /// </summary>
    public float Health = 100.0f;
    /// <summary>
    /// The minimum health to pass onto the second phase
    /// </summary>
    public float SecondPhaseHealthThreshold = 50.0f;

    /// <summary>
    /// Minimum time for a state change
    /// </summary>
    public float MinStateChangeTime = 2.0f;
    /// <summary>
    /// Maximum time for a state change
    /// </summary>
    public float MaxStateChangeTime = 4.0f;
    /// <summary>
    /// The playable area for the bosses
    /// </summary>
    public BoxCollider PlayableArea { get; set; }
    /// <summary>
    /// State machines of all the bosses created
    /// </summary>
    private BossStateMachine[] mBosses;

    /// <summary>
    /// Timer until next state change
    /// </summary>
    private float mStateChangeTimer = 0.0f;
    
    void Start ()
    {
        this.ResetChangeTimer ();
        
        // get access to both state machines as these will be directed from here
        this.mBosses = GetComponentsInChildren <BossStateMachine> ();
        this.PlayableArea = transform.Find ("PlayableArea").GetComponent <BoxCollider> ();
    }

    private void ResetChangeTimer ()
    {
        this.mStateChangeTimer = Random.Range (this.MinStateChangeTime, this.MaxStateChangeTime);
    }

    void FixedUpdate ()
    {
        this.mStateChangeTimer -= Time.fixedDeltaTime;
        
        if (this.mStateChangeTimer > 0.0f)
            return;

        this.ResetChangeTimer ();
        
        foreach (BossStateMachine machine in this.mBosses)
            machine.RandomNextState ();
    }
}
