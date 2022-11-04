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
    /// Distance to the object to follow
    /// </summary>
    public float Distance = 25.0f;

    void Update()
    {
        if (this.Follow is null)
            return;
        
        transform.position = Follow.position + (Follow.up * Distance) + -Vector3.forward * Distance;
        transform.LookAt(Follow.position);
    }
}
