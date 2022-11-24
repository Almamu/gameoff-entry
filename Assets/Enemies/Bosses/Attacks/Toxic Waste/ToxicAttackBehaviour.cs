using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicAttackBehaviour : MonoBehaviour
{
    /// <summary>
    /// The movement speed
    /// </summary>
    public float Speed = 1.1f;
    /// <summary>
    /// The destination of the movement
    /// </summary>
    public Vector3 Target;
    /// <summary>
    /// The height of the arc at the highest point
    /// </summary>
    public float ArcHeight = 5.0f;
    /// <summary>
    /// The time it takes for the waste to disappear
    /// </summary>
    [HideInInspector]
    public float DisappearanceTimer = 5.0f;
    /// <summary>
    /// The size of the waste
    /// </summary>
    [HideInInspector]
    public float Scale = 1.0f;
    [HideInInspector]
    public float Radius = 2f;
    /// <summary>
    /// The waste to activate when the attack lands
    /// </summary>
    public ToxicWasteBehaviour Waste { get; set; }
    
    /// <summary>
    /// The starting position
    /// </summary>
    public Vector3 StartPosition { get; set; }
    public float FinalSpeed { get; set; }
    /// <summary>
    /// The progress of movement through the arc (0 -> start, 1 -> end)
    /// </summary>
    private float mProgress;

    void Awake()
    {
        this.Waste = GetComponentInChildren <ToxicWasteBehaviour> (true);
    }

    void FixedUpdate()
    {
        // update the progress based off the speed and time passed
        this.mProgress = Mathf.Min(this.mProgress + Time.fixedDeltaTime * this.FinalSpeed, 1.0f);

        // convert the progress into a 0-1 value that indicates the current height point
        // (used as multiplier of the ArcHeight to get the current height) 
        float parabola = 1.0f - 4.0f * (this.mProgress - 0.5f) * (this.mProgress - 0.5f);

        // get the current position through the movement, linearly, ignoring the ight        
        Vector3 nextPos = Vector3.Lerp(this.StartPosition, this.Target, this.mProgress);

        // get the height using the parabola and the highest point in the arc
        nextPos.y += parabola * this.ArcHeight;

        // set the position
        transform.position = nextPos;

        // check if the movement is finished and do whatever else is needed after
        if (this.mProgress < 1.0f)
            return;

        // activate waste, keep information and unparent it
        this.Waste.OriginalLocalPosition = this.Waste.transform.localPosition;
        this.Waste.OriginalParent = this.transform;
        
        // set the time
        this.Waste.DisappearanceTimer = this.DisappearanceTimer;

        this.Waste.transform.localScale = new Vector3 (
            this.Scale,
            1.0f,
            this.Scale
        );

        this.Waste.transform.position = nextPos;
        this.Waste.transform.SetParent (transform.parent);
        this.Waste.gameObject.SetActive (true);
        
        this.Waste.SphereCollider.radius = this.Radius;
        
        // reset progress and positions
        this.mProgress = 0;
        
        // finally deactivate ourselves
        this.gameObject.SetActive (false);
    }
}
