using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIndicatorBehaviour : MonoBehaviour
{
    public Camera Camera;
    public Transform Objective;

    private RectTransform Transform;

    void Start ()
    {
        this.Transform = GetComponent <RectTransform> ();
    }
    void Update()
    {
        if (this.Objective.gameObject.activeInHierarchy == false)
        {
            // move it offscreen
            this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Bottom, this.Camera.pixelHeight, this.Transform.rect.height);
            this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, this.Camera.pixelWidth, this.Transform.rect.width);
            return;
        }
        
        Vector3 screenPos = this.Camera.WorldToScreenPoint (this.Objective.position);

        if (screenPos.x >= 0 && screenPos.x <= this.Camera.pixelWidth && screenPos.y >= 0 && screenPos.y <= this.Camera.pixelHeight)
        {
            // move it offscreen
            this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Bottom, this.Camera.pixelHeight, this.Transform.rect.height);
            this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, this.Camera.pixelWidth, this.Transform.rect.width);
            return;
        }
        
        screenPos.x -= (float) this.Camera.pixelWidth / 2;
        screenPos.y -= (float) this.Camera.pixelHeight / 2;
        screenPos.z = 0.0f;
        
        screenPos = screenPos.normalized * 150;
        
        // extract angle from the direction
        this.Transform.rotation = Quaternion.LookRotation (Vector3.forward, screenPos);
        
        this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Bottom, screenPos.y, this.Transform.rect.height);
        this.Transform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, screenPos.x, this.Transform.rect.width);
    }
}
