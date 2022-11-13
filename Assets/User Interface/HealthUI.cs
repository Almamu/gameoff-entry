using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    /// <summary>
    /// The fill image used to represent the health
    /// </summary>
    private Image mFillImage;
    
    void Start()
    {
        this.mFillImage = GetComponent <Image> ();
        // subscribe to health changes
        PlayerStateMachine.HealthUpdate += this.OnHealthUpdate;
    }

    void OnHealthUpdate (float newAmount)
    {
        this.mFillImage.fillAmount = newAmount;
    }
}
