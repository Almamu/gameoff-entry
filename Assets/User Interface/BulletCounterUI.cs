using UnityEngine;
using UnityEngine.UI;

public class BulletCounterUI : MonoBehaviour
{
    private Image [] mIcons;
    
    void Start ()
    {
        this.mIcons = GetComponentsInChildren <Image> ();
        // subscribe to the player's event
        PlayerMovementState.OnReload += this.OnAmmunitionUpdate;
        PlayerMovementState.OnShoot += this.OnAmmunitionUpdate;
    }

    void OnAmmunitionUpdate (int newCount)
    {
        // hide all icons and show the only ones required
        for (int i = 0; i < this.mIcons.Length; i++)
            this.mIcons [i].enabled = i < newCount;
    }
}
