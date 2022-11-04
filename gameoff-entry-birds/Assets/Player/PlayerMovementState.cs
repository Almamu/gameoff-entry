using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementState : PlayerState
{
    public float MovementSpeed = 500.0f;
    public float InterpolationSpeed = 0.1f;
    
    private float mHorizontal = 0.0f;
    private float mVertical = 0.0f;

    private Vector3 mLookAtPoint;

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotationToCamera();
        
        if (Input.GetButton ("Fire") == true)
            HandleShooting();
        if (Input.GetButton ("Dodge") == true)
            HandleDodge ();
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
        
        // enable the bullet
        bullet.SetActive(true);
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
