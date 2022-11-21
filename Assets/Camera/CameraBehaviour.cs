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

        Vector3 position = this.Follow.position;
        Vector3 newPosition = position + (Follow.up * this.VerticalDistance) + -Vector3.forward * this.HorizontalDistance;

        transform.position = newPosition;
        transform.LookAt(position);
    }
}
