using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManekinController : MonoBehaviour
{
    public TutorialSceneController Controller;

    public GameObject Player;

    public GameObject ShootPrefab;
    
    public float ShootingIntervals;

    private float mTimer;

    private bool mDisabled = false;

    private static int NumberOfManekinsDestroyed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        NumberOfManekinsDestroyed = 0;
        this.mTimer = 0;
        // disable ourselves on movement disabled
        CombatEventManager.DisableMovement += this.MovementDisabled;
        CombatEventManager.EnableMovement += this.MovementEnabled;
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
        if (this.Controller.IsFirstStageDone () == false || this.mDisabled == true)
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
    
    private void OnTextBoxHidden ()
    {
        CombatEventManager.ClearEvents ();
        // go to next scene
        SceneManager.LoadScene ("Playground Cutscene");
    }
    
    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.CompareTag ("Player Bullet") == false)
            return;
        
        Destroy (this.gameObject);

        NumberOfManekinsDestroyed++;

        if (NumberOfManekinsDestroyed >= 6)
        {
            CombatEventManager.EnableMovement += this.OnTextBoxHidden;
            CombatEventManager.InvokeTextbox ("SARGE.RANT1", MessageSource.Sarge);
            CombatEventManager.InvokeTextbox ("SARGE.RANT.ANSWER", MessageSource.Player);
            CombatEventManager.InvokeTextbox ("SARGE.RANT5", MessageSource.Sarge);
            CombatEventManager.InvokeTextbox ("SARGE.RANT6", MessageSource.Sarge);
            return;
        }
        

        if (this.Controller.IsFirstStageDone () == true)
            return;
        
        this.Controller.SetFirstStageDone ();
        
        // show the message for dodging
        CombatEventManager.InvokeTextbox ("TUTORIAL.DODGE1", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("TUTORIAL.DODGE2", MessageSource.Sarge);
    }
}
