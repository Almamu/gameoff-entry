using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
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
    /// Event fired when a textbox has to be displayed on-screen
    /// </summary>
    public static event Action<string> Textbox;

    public static void InvokeDisableMovement()
    {
        DisableMovement?.Invoke();
    }

    public static void InvokeEnableMovement()
    {
        EnableMovement?.Invoke();
    }

    public static void InvokeTextbox(string message)
    {
        Textbox?.Invoke(message);
    }
}
