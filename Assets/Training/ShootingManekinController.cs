using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManekinController : MonoBehaviour
{
    public GameObject Player;

    public GameObject [] Manekins;

    public GameObject ShootPrefab;
    
    public float ShootingIntervals;

    private float mTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        this.mTimer = this.ShootingIntervals;
    }

    // Update is called once per frame
    void Update()
    {
        bool destroyed = false;
        
        // check if any manekin is destroyed and start shooting
        foreach (GameObject manekin in this.Manekins)
        {
            if (manekin == null)
            {
                destroyed = true;
                break;
            }
        }

        if (destroyed == false)
            return;
        
        // do the thing
        this.mTimer -= Time.deltaTime;

        if (this.mTimer > 0)
            return;

        this.mTimer = this.ShootingIntervals;
        
        // create a bullet, set it in motion, and go
        Instantiate (
            this.ShootPrefab,
            transform.position,
            Quaternion.LookRotation ((this.Player.transform.position - transform.position).normalized)
        );
    }
}
