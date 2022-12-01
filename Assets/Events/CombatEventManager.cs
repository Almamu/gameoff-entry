using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatEventManager : MonoBehaviour
{
    /// <summary>
    /// Event fired when the player control should be disabled
    /// </summary>
    public static event Action DisableMovement;

    /// <summary>
    /// Event fired when the player control should be enabled
    /// </summary>
    public static event Action EnableMovement;

    /// <summary>
    /// Event fired when the boss' life changes
    /// </summary>
    public static event Action <float> BossHealth;

    /// <summary>
    /// Event fired when a textbox has to be displayed on-screen
    /// </summary>
    public static event Action<string, MessageSource> Textbox;
    /// <summary>
    /// Event fired when the textbox is hidden
    /// </summary>
    public static event Action TextboxHidden;

    public static void InvokeDisableMovement()
    {
        DisableMovement?.Invoke();
    }

    public static void InvokeEnableMovement()
    {
        EnableMovement?.Invoke();
    }

    public static void InvokeTextbox(string message, MessageSource source = MessageSource.Other)
    {
        Textbox?.Invoke(message, source);
    }

    public static void InvokeBossHealth (float healthLeft)
    {
        BossHealth?.Invoke (healthLeft);
    }

    public static void InvokeTextboxHidden ()
    {
        TextboxHidden?.Invoke ();
    }

    public static void ClearEvents ()
    {
        TextboxHidden = null;
        BossHealth = null;
        EnableMovement = null;
        DisableMovement = null;
        Textbox = null;
    }
}
