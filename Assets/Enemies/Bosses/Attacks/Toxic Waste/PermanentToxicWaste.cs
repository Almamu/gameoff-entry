using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentToxicWaste : MonoBehaviour
{
    private void OnTriggerStay (Collider collision)
    {
        if (this.enabled == false || collision.gameObject.CompareTag ("Player") == false)
            return;

        collision.gameObject.SendMessage ("ApplyToxicDamage");
    }
}
