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
    public float ShootingCooldown = 0.5f;
    /// <summary>
    /// Adds some angle to the shooting so not all bullets go straight
    /// </summary>
    public float ShootingJitter = 1.0f;
    /// <summary>
    /// The time it takes to reload
    /// </summary>
    public float ClipReloadTime = 1.0f;
    /// <summary>
    /// The amount of bullets a magazine/clip has
    /// </summary>
    public int ClipBullets = 8;
    /// <summary>
    /// The time it takes for the dodge to cooldown
    /// </summary>
    public float DodgeCooldown = 2.0f;
    /// <summary>
    /// The time the player stops moving for shooting
    /// </summary>
    public float ShootBlockTime = 0.075f;
    
    /// <summary>
    /// Horizontal position of the stick
    /// </summary>
    private float mHorizontal = 0.0f;
    /// <summary>
    /// Vertical position of the stick
    /// </summary>
    private float mVertical = 0.0f;
    /// <summary>
    /// The shooting cooldown timer
    /// </summary>
    private float mShootingCooldown = 0.0f;
    /// <summary>
    /// The reload timer
    /// </summary>
    private float mReloadTimer = 0.0f;
    /// <summary>
    /// The dodge cooldown timer
    /// </summary>
    private float mDodgeCooldownTimer = 0.0f;

    private Vector3 mLookAtPoint;

    /// <summary>
    /// The current amount of bullets inside the clip
    /// </summary>
    private int mClipBullets;

    private float mShootBlock = 0.0f;

    /// <summary>
    /// Event fired when the player shoots
    /// </summary>
    public static event Action<int> OnShoot;

    /// <summary>
    /// Event fired when the player finishes reloading
    /// </summary>
    public static event Action <int> OnReload;

    private void OnEnable ()
    {
        // TODO: PLAY SOUND
        
        // reload the magazine/clip (when enabled, so this applies to state change and other events that enable/disable components)
        this.mClipBullets = this.ClipBullets;

        OnReload?.Invoke (this.mClipBullets);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotationToCamera();
        
        if (Input.GetButton ("Fire") == true)
            HandleShooting();
        if (Input.GetButton ("Dodge") == true)
            HandleDodge ();

        HandleCooldown ();
        HandleReload ();
    }

    private void HandleMovement()
    {
        this.mHorizontal = math.lerp (this.mHorizontal, Input.GetAxis("Horizontal"), InterpolationSpeed);
        this.mVertical = math.lerp (this.mVertical, Input.GetAxis("Vertical"), InterpolationSpeed);

        Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);

        if (this.mShootBlock >= this.ShootBlockTime)
        {
            movement = new Vector3(
                this.mHorizontal * MovementSpeed,
                this.Rigidbody.velocity.y, // keep vertical velocity
                this.mVertical * MovementSpeed
            );
        }

        this.Rigidbody.velocity = movement;

        // calculate the direction we're looking at and set the right variables in the animator
        Vector3 localDirection = transform.InverseTransformDirection (movement);
        
        if (localDirection == Vector3.zero)
        {
            this.Machine.ModelAnimator.SetBool (PlayerStateMachine.WalkingId, false);
            
            return;
        }

        this.Machine.ModelAnimator.SetBool (PlayerStateMachine.WalkingId, true);
        this.Machine.ModelAnimator.SetFloat (
            PlayerStateMachine.AngleId,
            Quaternion.LookRotation (localDirection).eulerAngles.y
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
        if (this.mShootingCooldown < this.ShootingCooldown || this.mReloadTimer > 0.0f)
            return;
        
        // get the bullet from the pool
        GameObject bullet = this.BulletPool.Pop();

        if (bullet is null)
        {
            Debug.LogError("Bullet pool was exhausted");
            return;
        }

        Transform t = transform;

        // setup the bullet
        bullet.transform.SetPositionAndRotation (
            t.position + t.forward,
            t.rotation
        );

        // some jitter so not all bullets go the same direction
        bullet.transform.Rotate (Vector3.up, Random.Range (-this.ShootingJitter, this.ShootingJitter));
        
        // enable the bullet
        bullet.SetActive(true);
        
        // decrease ammunition count
        this.mClipBullets--;
        
        // reset shooting cooldown
        this.mShootingCooldown = 0.0f;
        // set the movement lock timer
        this.mShootBlock = 0.0f;
        
        // fire the onshoot event
        OnShoot?.Invoke(this.mClipBullets);
        
        // play the shoot sound
        this.Machine.AudioSource.PlayOneShot (this.Machine.ShootSound);

        if (this.mClipBullets > 0)
            return;
        
        // play the reload sound
        this.Machine.AudioSource.PlayOneShot (this.Machine.ReloadSound);
        // set the reload timer
        this.mReloadTimer = this.ClipReloadTime;
    }

    private void HandleCooldown ()
    {
        // increase the cooldown timers
        if (this.mShootingCooldown < this.ShootingCooldown)
            this.mShootingCooldown += Time.fixedDeltaTime;
        if (this.mDodgeCooldownTimer < this.DodgeCooldown)
            this.mDodgeCooldownTimer += Time.fixedDeltaTime;
        if (this.mShootBlock < this.ShootBlockTime)
            this.mShootBlock += Time.fixedDeltaTime;
    }

    private void HandleReload ()
    {
        // reload only needs to be handled if the timer is not yet depleted
        if (this.mReloadTimer <= 0.0f)
            return;

        // decrease timer
        this.mReloadTimer -= Time.fixedDeltaTime;

        // only fire the reload event if the timer is finished
        if (this.mReloadTimer > 0.0f)
            return;

        this.mClipBullets = this.ClipBullets;
        
        OnReload?.Invoke (this.mClipBullets);
    }

    private void HandleDodge()
    {
        if (this.mDodgeCooldownTimer < this.DodgeCooldown)
            return;
        
        Vector3 direction = this.Rigidbody.velocity.normalized;
        
        if (math.abs (this.mHorizontal) < 0.1 && math.abs (this.mVertical) < 0.1)
            direction = gameObject.transform.forward;

        direction.y = 0;

        this.DodgeState.Direction = direction;
        this.mDodgeCooldownTimer = 0.0f;
            
        this.Machine.PushState(this.DodgeState);
    }
}
