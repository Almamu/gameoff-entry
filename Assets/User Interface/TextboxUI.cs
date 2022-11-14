using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextboxUI : MonoBehaviour
{
    /// <summary>
    /// The amount of time between each character appearing
    /// </summary>
    public float TextDisplayDelay = 0.2f;
    /// <summary>
    /// The amount of characters will appear each time
    /// </summary>
    public int DisplayCharactersAtOnce = 2;
    /// <summary>
    /// Queue of text to show in the textboxes
    /// </summary>
    private Queue <string> mMessageQueue = new Queue <string> ();
    /// <summary>
    /// The message currently displaying
    /// </summary>
    private string mCurrentMessage = "";
    /// <summary>
    /// The current text of the message visible
    /// </summary>
    private string mMessageProgress = "";
    /// <summary>
    /// Whether the textbox is being displayed or not
    /// </summary>
    private bool mDisplayingMessage = false;

    private TextMeshProUGUI mText;
    private float mTimer = 0.0f;
    
    void Start()
    {
        // bind to specific events
        EventManager.Textbox += this.QueueTextbox;
        // get elements inside
        this.mText = GetComponentInChildren <TextMeshProUGUI> (true);
    }

    void FixedUpdate ()
    {
        // do not do anything if there's no textbox displayed
        if (this.mDisplayingMessage == false)
            return;

        if (Input.GetButton ("Fire") == true)
        {
            // dequeu message and go to the next
            this.DequeueMessage ();
        }

        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;
        
        // get next X characters
        this.mMessageProgress =
            this.mCurrentMessage.Substring (
                0, Math.Min (this.mMessageProgress.Length + this.DisplayCharactersAtOnce, this.mCurrentMessage.Length - 1)
            );
        
        // update text
        this.mText.text = this.mMessageProgress;
        
        // reset the timer
        this.mTimer = this.TextDisplayDelay;
    }

    void DequeueMessage ()
    {
        if (this.mMessageQueue.Count == 0)
        {
            this.mDisplayingMessage = false;
            // disable textbox container
            this.mText.gameObject.SetActive (false);
            // enable movement again
            EventManager.InvokeEnableMovement ();
            // TODO: HIDE EVERYTHING RELATED TO THE UI
            return;
        }
        
        // ensure the textbox is active and has information
        this.mDisplayingMessage = true;
        this.mCurrentMessage = this.mMessageQueue.Dequeue ();
        this.mMessageProgress = "";
        this.mText.text = "";
        // enable the textbox container
        this.mText.gameObject.SetActive (true);
        // set the timer
        this.mTimer = this.TextDisplayDelay;
    }

    void QueueTextbox (string message)
    {
        // disable movement for everything
        EventManager.InvokeDisableMovement ();
        // queue the message
        this.mMessageQueue.Enqueue (message);
        
        // only dequeue a message if the textbox is not visible yet
        if (this.mDisplayingMessage == false)
            this.DequeueMessage ();
    }
}
