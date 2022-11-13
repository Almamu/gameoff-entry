using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVectorBehaviour : MonoBehaviour
{
    /// <summary>
    /// Maximum distance the direction vector will be
    /// </summary>
    public float MaxDistance = 2.0f;

    public Rigidbody RigidbodyToFollow;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = this.RigidbodyToFollow.velocity.normalized;

        // y position shouldn't be taken into account
        velocity.y = 0;
        
        this.transform.position = Vector3.Lerp(
            this.transform.position,
            this.RigidbodyToFollow.position + (velocity * MaxDistance),
            0.005f
        );
    }
}
