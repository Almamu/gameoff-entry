using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManekinController : MonoBehaviour
{
    public GameObject Player;

    public GameObject ShootPrefab;
    
    public float ShootingIntervals;

    private ManekinController Base;
    
    private float mTimer;

    private bool mDisabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        this.Base = GetComponent <ManekinController> ();
        this.mTimer = this.ShootingIntervals;
        // disable ourselves on movement disabled
        CombatEventManager.DisableMovement += this.MovementDisabled;
        CombatEventManager.EnableMovement += this.MovementEnabled;
    }

    private void OnDestroy ()
    {
        CombatEventManager.DisableMovement -= this.MovementDisabled;
        CombatEventManager.EnableMovement -= this.MovementEnabled;
    }

    void MovementDisabled ()
    {
        this.mDisabled = true;
    }

    void MovementEnabled ()
    {
        this.mDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.Base.Controller.IsFirstStageDone () == false || this.mDisabled == true)
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
