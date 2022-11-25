using System;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float DisappearTime = 5.0f;
    public float ShootSpeed = 25.0f;
    private float mCurrentTime;
    private Rigidbody mRigidbody;

    void OnEnable ()
    {
        this.mCurrentTime = this.DisappearTime;
        if (this.mRigidbody is not null)
            this.mRigidbody.velocity = transform.forward * this.ShootSpeed;
    }
    
    void Start()
    {
        this.mRigidbody = GetComponent <Rigidbody> ();
        
        // subscribe to required events to alter state
        EventManager.DisableMovement += OnDisableMovement;
        EventManager.EnableMovement += OnEnableMovement;
    }

    void FixedUpdate ()
    {
        if (this.enabled == false)
            return;
        
        this.mRigidbody.velocity = transform.forward * this.ShootSpeed;

        this.mCurrentTime -= Time.fixedDeltaTime;

        if (this.mCurrentTime > 0.0f)
            return;

        gameObject.SetActive (false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive (false);
    }
    
    void OnEnableMovement ()
    {
        this.enabled = true;
    }

    void OnDisableMovement ()
    {
        this.enabled = false;
    }

}
