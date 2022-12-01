using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    /// <summary>
    /// The fill image used to represent the health
    /// </summary>
    private Image mFillImage;
    
    // TODO: MAYBE EASE CHANGES?

    void Start()
    {
        this.mFillImage = GetComponent <Image> ();
        // subscribe to health changes
        PlayerStateMachine.StaminaUpdate += this.OnStaminaUpdate;
    }

    void OnStaminaUpdate (float newAmount)
    {
        this.mFillImage.fillAmount = newAmount;

        if (newAmount >= 1.0f)
        {
            this.mFillImage.color = new Color (1f, 0.7f, 0);
        }
        else
        {
            this.mFillImage.color = Color.white;
        }
    }
}
