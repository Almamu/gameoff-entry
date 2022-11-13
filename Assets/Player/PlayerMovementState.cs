using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovementState : PlayerState
{
    /// <summary>
    /// The player's movement speed
    /// </summary>
    public float MovementSpeed = 5.0f;
    /// <summary>
    /// The speed at which the player gains speed
    /// </summary>
    public float InterpolationSpeed = 1.0f;
    /// <summary>
    /// The amount of time shooting needs to cooldown
    /// </summary>
    public float ShootingCooldown = 0.1f;
    /// <summary>
    /// Adds some angle to the shooting so not all bullets go straight
    /// </summary>
    public float ShootingJitter = 1.0f;
    /// <summary>
    /// The time it takes to reload
    /// </summary>
    public float ClipReloadTime = 2.0f;

    public int ClipBullets = 10;
    
    private float mHorizontal = 0.0f;
    private float mVertical = 0.0f;
    private float mShootingCooldown = 0.0f;

    private Vector3 mLookAtPoint;

    private int mClipBullets;

    /// <summary>
    /// Event fired when the player shoots
    /// </summary>
    public static event Action<int> OnShoot;

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotationToCamera();
        
        if (Input.GetButton ("Fire") == true)
            HandleShooting();
        if (Input.GetButton ("Dodge") == true)
            HandleDodge ();
        
        // increase the cooldown timers
        this.mShootingCooldown += Time.fixedDeltaTime;
    }

    private void HandleMovement()
    {
        this.mHorizontal = math.lerp (this.mHorizontal, Input.GetAxis("Horizontal"), InterpolationSpeed);
        this.mVertical = math.lerp (this.mVertical, Input.GetAxis("Vertical"), InterpolationSpeed);

        this.Rigidbody.velocity = new Vector3(
            this.mHorizontal * MovementSpeed,
            this.Rigidbody.velocity.y, // keep vertical velocity
            this.mVertical * MovementSpeed
        );
    }

    private void HandleRotationToCamera()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = gameObject.transform.position.y;
        
        Ray raycast = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(raycast, out RaycastHit hit) != true)
            return;
        
        this.mLookAtPoint = hit.point;

        Vector3 lookPos = this.mLookAtPoint - gameObject.transform.position;
        lookPos.y = 0;

        gameObject.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void HandleShooting()
    {
        // prevent shooting if it's not cooled down
        if (this.mShootingCooldown < this.ShootingCooldown)
            return;
        
        // get the bullet from the pool
        GameObject bullet = this.BulletPool.Pop();

        if (bullet is null)
        {
            Debug.LogError("Bullet pool was exhausted");
            return;
        }

        // setup the bullet
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        // some jitter so not all bullets go the same direction
        bullet.transform.Rotate (Vector3.up, Random.Range (-this.ShootingJitter, this.ShootingJitter));
        
        // enable the bullet
        bullet.SetActive(true);
        
        // fire the onshoot event
        OnShoot?.Invoke(1);
    }

    private void HandleDodge()
    {
        Vector3 direction = this.Rigidbody.velocity.normalized;
        
        if (math.abs (this.mHorizontal) < 0.1 && math.abs (this.mVertical) < 0.1)
            direction = gameObject.transform.forward;

        direction.y = 0;

        this.DodgeState.Direction = direction;
            
        this.Machine.PushState(this.DodgeState);
    }
}
