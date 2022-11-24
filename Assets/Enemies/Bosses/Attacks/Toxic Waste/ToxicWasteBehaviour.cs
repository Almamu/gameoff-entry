using System;
using UnityEngine;

public class ToxicWasteBehaviour : MonoBehaviour
{
    /// <summary>
    /// The time it takes for the toxic waste to disappear
    /// </summary>
    [HideInInspector]
    public float DisappearanceTimer = 5.0f;
    /// <summary>
    /// Timer used to prevent activation from taking too much time
    /// </summary>
    public float ActivationTimer = 8.0f;
    /// <summary>
    /// Used when resetting for the pool
    /// </summary>
    public Vector3 OriginalLocalPosition { get; set; }
    /// <summary>
    /// Used when resetting for the pool
    /// </summary>
    public Transform OriginalParent { get; set; }
    /// <summary>
    /// The rigidbody the toxic waste uses to find the ground
    /// </summary>
    private Rigidbody mRigidbody;
    /// <summary>
    /// Box collider used for detection of ground
    /// </summary>
    private BoxCollider mBoxCollider;
    /// <summary>
    /// Sphere collider used as trigger for the player's damage
    /// </summary>
    public SphereCollider SphereCollider;

    /// <summary>
    /// The collision layer to move to when the object is placed at it's final position
    /// </summary>
    private static int DefaultCollisionLayer;
    /// <summary>
    /// The collision layer to move to when the object is reseted to it's initial state
    /// </summary>
    private int mStartupLayer;
    /// <summary>
    /// Current timer value
    /// </summary>
    private float mTimer = 0;
    /// <summary>
    /// Timer used to prevent taking too much on activation
    /// </summary>
    private float mActivationTimer = 0.0f;
    /// <summary>
    /// Whether the timer should be activated or not
    /// </summary>
    private bool mActivated = false;
    
    void Awake()
    {
        this.mStartupLayer = gameObject.layer;
        this.mActivationTimer = this.ActivationTimer;
        DefaultCollisionLayer = LayerMask.NameToLayer ("Default");
        
        this.mRigidbody = this.GetComponent <Rigidbody> ();
        this.mBoxCollider = this.GetComponent <BoxCollider> ();
        this.SphereCollider = this.GetComponent <SphereCollider> ();
    }

    private void OnCollisionEnter (Collision collision)
    {
        // collided means rigidbody can be deactivated and real collider enabled
        this.mRigidbody.isKinematic = true;
        this.mBoxCollider.enabled = false;
        this.SphereCollider.enabled = true;
        
        // set the collision layer now
        this.gameObject.layer = DefaultCollisionLayer;

        this.mTimer = this.DisappearanceTimer;
        this.mActivated = true;
    }

    private void HandleActivationTimer ()
    {
        if (this.mActivated == true)
            return;

        this.mActivationTimer -= Time.fixedDeltaTime;
        
        if (this.mActivationTimer <= 0.0f)
            this.ResetToPool ();
    }

    private void HandleMovement ()
    {
        if (this.mActivated == false)
            return;

        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;
        
        this.ResetToPool ();
    }

    private void ResetToPool ()
    {
        // reparent and reset position
        this.transform.SetParent (this.OriginalParent);
        this.transform.localPosition = this.OriginalLocalPosition;
        
        // reset rigidbody and colliders
        this.mRigidbody.isKinematic = false;
        this.mBoxCollider.enabled = true;
        this.SphereCollider.enabled = false;

        this.gameObject.layer = this.mStartupLayer;
        
        // finally reset activation state
        this.mActivated = false;

        this.gameObject.SetActive (false);
    }

    private void FixedUpdate ()
    {
        this.HandleActivationTimer ();
        this.HandleMovement ();
    }
}
