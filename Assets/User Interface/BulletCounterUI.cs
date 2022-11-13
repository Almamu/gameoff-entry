using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCounterUI : MonoBehaviour
{
    // TODO: CHANGE TO SOME KIND OF ICON
    private TextMeshProUGUI mText;
    
    void Start ()
    {
        this.mText = GetComponent <TextMeshProUGUI> ();
        // subscribe to the player's event
        PlayerMovementState.OnReload += this.OnAmmunitionUpdate;
        PlayerMovementState.OnShoot += this.OnAmmunitionUpdate;
    }

    void OnAmmunitionUpdate (int newCount)
    {
        this.mText.text = newCount.ToString ();
    }
}
