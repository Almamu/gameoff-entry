using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManekinController : MonoBehaviour
{
    public TutorialSceneController Controller;

    private void OnTextBoxHidden ()
    {
        CombatEventManager.EnableMovement -= this.OnTextBoxHidden;
        // go to next scene
        SceneManager.LoadScene ("Scenes/Playground");
    }
    
    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.CompareTag ("Player Bullet") == false)
            return;

        if (this.gameObject.CompareTag ("Shooting Manekin") == true && this.Controller.IsFirstStageDone () == true)
        {
            CombatEventManager.EnableMovement += this.OnTextBoxHidden;
            CombatEventManager.InvokeTextbox ("SARGE.RANT1", MessageSource.Sarge);
            CombatEventManager.InvokeTextbox ("SARGE.RANT.ANSWER", MessageSource.Player);
            CombatEventManager.InvokeTextbox ("SARGE.RANT5", MessageSource.Sarge);
            CombatEventManager.InvokeTextbox ("SARGE.RANT6", MessageSource.Sarge);
            return;
        }
        
        if (this.gameObject.CompareTag ("Normal Manekin") == false)
            return;
        
        Destroy (this.gameObject);

        if (this.Controller.IsFirstStageDone () == true)
            return;
        
        this.Controller.SetFirstStageDone ();
        
        // show the message for dodging
        CombatEventManager.InvokeTextbox ("TUTORIAL.DODGE1", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("TUTORIAL.DODGE2", MessageSource.Sarge);
    }
}
