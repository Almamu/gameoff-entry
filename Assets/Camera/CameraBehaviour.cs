using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    /// <summary>
    /// Object the camera should follow
    /// </summary>
    public Transform Follow;
    /// <summary>
    /// Vertical distance to the object to follow
    /// </summary>
    public float VerticalDistance = 25.0f;
    /// <summary>
    /// Horizontal distance to the object to follow
    /// </summary>
    public float HorizontalDistance = 25.0f;

    void Update()
    {
        if (this.Follow is null)
            return;
        
        transform.position = Follow.position + (Follow.up * this.VerticalDistance) + -Vector3.forward * this.HorizontalDistance;
        transform.LookAt(Follow.position);
    }
}
