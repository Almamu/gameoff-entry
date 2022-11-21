using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallRacimoAttackBehaviour : MonoBehaviour
{
    /// <summary>
    /// The speed at which this attack moves
    /// </summary>
    public float MovementSpeed = 0.3f;

    /// <summary>
    /// The time it takes for the object to disappear
    /// </summary>
    public float DestroyTimer = 8.0f;

    /// <summary>
    /// The starting parent for the racimo
    /// </summary>
    public Transform StartParent { get; set; }
    /// <summary>
    /// The starting local position for the racimo
    /// </summary>
    public Vector3 StartLocalPosition { get; set; }
    /// <summary>
    /// The timer to control destroyal of this bullet
    /// </summary>
    private float mTimer = 0.0f;

    void Awake ()
    {
        this.mTimer = this.DestroyTimer;
    }

    void FixedUpdate()
    {
        transform.Translate (Vector3.forward * this.MovementSpeed);

        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;

        // reset things
        this.transform.SetParent (this.StartParent);
        this.transform.localPosition = this.StartLocalPosition;
        this.mTimer = this.DestroyTimer;
        this.gameObject.SetActive (false);
    }
}
