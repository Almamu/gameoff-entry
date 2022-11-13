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
        Vector3 direction = this.mRigidbody.velocity.normalized;

        // y position shouldn't be taken into account
        direction.y = 0;
        
        DirectionVector.transform.position = Vector3.Lerp (
            DirectionVector.transform.position,
            transform.position + (direction * MaxDistance),
            0.05f
        );
    }
}
