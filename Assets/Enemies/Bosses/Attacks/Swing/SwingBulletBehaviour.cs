using System;
using UnityEngine;

public class SwingBulletBehaviour : MonoBehaviour
{
    public float DisappearTime = 5.0f;
    public float ShootSpeed = 25.0f;
    private float mCurrentTime;
    private Rigidbody mRigidbody;

    void OnEnable ()
    {
        this.mCurrentTime = this.DisappearTime;
    }
    
    void Start()
    {
        this.mRigidbody = GetComponent <Rigidbody> ();
        
        // subscribe to required events to alter state
        CombatEventManager.DisableMovement += OnDisableMovement;
        CombatEventManager.EnableMovement += OnEnableMovement;
    }

    void FixedUpdate ()
    {
        if (this.enabled == false)
            return;
        
        this.mRigidbody.velocity = transform.forward * ShootSpeed;

        this.mCurrentTime -= Time.fixedDeltaTime;

        if (this.mCurrentTime > 0.0f)
            return;

        gameObject.SetActive (false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive (false);
        
        if (this.enabled == false || collision.gameObject.CompareTag ("Player") == false)
            return;

        collision.gameObject.SendMessage ("ApplySwingDamage");
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
