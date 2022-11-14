using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TextboxUI : MonoBehaviour
{
    /// <summary>
    /// The amount of time between each character appearing
    /// </summary>
    public float TextDisplayDelay = 0.1f;
    /// <summary>
    /// The amount of characters will appear each time
    /// </summary>
    public int DisplayCharactersAtOnce = 1;
    /// Queue of text to show in the textboxes
    /// </summary>
    private Queue <string> mMessageQueue = new Queue <string> ();
    /// <summary>
    /// Whether the textbox is being displayed or not
    /// </summary>
    private bool mDisplayingMessage = false;

    private TextMeshProUGUI mText;
    private float mTimer = 0.0f;

    private AudioSource mAudioSource;
    
    void Start()
    {
        // bind to specific events
        EventManager.Textbox += this.QueueTextbox;
        // get the text container
        this.mText = GetComponentInChildren <TextMeshProUGUI> (true);
        // get the audio source so we can play sounds
        this.mAudioSource = GetComponent <AudioSource> ();
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

        // do not do anything timer-related if enough characters are shown
        if (this.mText.maxVisibleCharacters >= this.mText.text.Length)
            return;
        
        this.mTimer -= Time.fixedDeltaTime;

        if (this.mTimer > 0.0f)
            return;
        
        // get next X characters
        this.mText.maxVisibleCharacters += this.DisplayCharactersAtOnce;
        
        // set a random pitch based off 1
        this.mAudioSource.pitch = Random.Range (0.9f, 1.1f);
        // play the audio
        this.mAudioSource.Play ();
        
        // reset the timer
        this.mTimer = this.TextDisplayDelay;
    }

    void SetChildrenActive (bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild (i).gameObject.SetActive (active);
    }

    void DequeueMessage ()
    {
        if (this.mMessageQueue.Count == 0)
        {
            this.mDisplayingMessage = false;
            // disable everything inside us
            SetChildrenActive (false);
            // enable movement again
            EventManager.InvokeEnableMovement ();
            // TODO: HIDE EVERYTHING RELATED TO THE UI
            return;
        }
        
        // ensure the textbox is active and has information
        this.mDisplayingMessage = true;
        this.mText.text = this.mMessageQueue.Dequeue ();
        this.mText.maxVisibleCharacters = 0;
        // force mesh update
        this.mText.ForceMeshUpdate (forceTextReparsing: true);
        // set the timer
        this.mTimer = this.TextDisplayDelay;
        // enable everything inside us
        SetChildrenActive (true);
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
