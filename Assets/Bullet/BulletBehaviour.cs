using System;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float DisappearTime = 5.0f;
    public float ShootSpeed = 25.0f;
    private float mCurrentTime;
    private Rigidbody mRigidbody;

    void Start()
    {
        this.mRigidbody = GetComponent <Rigidbody> ();
    }
    
    // Update is called once per frame
    void Update()
    {
        this.mRigidbody.velocity = transform.forward * ShootSpeed;

        this.mCurrentTime += Time.deltaTime * Time.timeScale;

        if (this.mCurrentTime > DisappearTime)
        {
            gameObject.SetActive(false);
            // reset the timer for the next time
            this.mCurrentTime = 0.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.mCurrentTime = 0.0f;
        gameObject.SetActive(false);
    }
}
