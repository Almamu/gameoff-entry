using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVectorBehaviour : MonoBehaviour
{
    /// <summary>
    /// The game object to move
    /// </summary>
    public GameObject DirectionVector;

    /// <summary>
    /// Maximum distance the direction vector will be
    /// </summary>
    public float MaxDistance = 2.0f;

    private Rigidbody mRigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        this.mRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = this.mRigidbody.velocity.normalized;

        // y position shouldn't be taken into account
        velocity.y = 0;
        
        DirectionVector.transform.position = Vector3.Lerp(
            DirectionVector.transform.position,
            transform.position + (velocity * MaxDistance),
            0.1f
        );
    }
}
