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
    struct TextboxInformation
    {
        public string Message { get; set; }
        public Vector3 WorldPosition { get; set; }
    }
    
    /// <summary>
    /// The amount of time between each character appearing
    /// </summary>
    public float TextDisplayDelay = 0.1f;
    /// <summary>
    /// The amount of characters will appear each time
    /// </summary>
    public int DisplayCharactersAtOnce = 1;
    /// <summary>
    /// The maximum pitch shift for the voice
    /// </summary>
    public float PitchShift = 0.1f;
    /// Queue of text to show in the textboxes
    /// </summary>
    private Queue <TextboxInformation> mMessageQueue = new Queue <TextboxInformation> ();
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
        CombatEventManager.Textbox += this.QueueTextbox;
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

        if (Input.GetButtonDown ("Fire") == true)
        {
            // display all the text or go to the next one
            if (this.mText.maxVisibleCharacters < this.mText.text.Length)
                this.mText.maxVisibleCharacters = this.mText.text.Length;
            else
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
        this.mAudioSource.pitch = Random.Range (1.0f - this.PitchShift, 1.0f + this.PitchShift);
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
            CombatEventManager.InvokeEnableMovement ();
            return;
        }
        
        // ensure the textbox is active and has information
        this.mDisplayingMessage = true;
        TextboxInformation info = this.mMessageQueue.Dequeue ();

        this.mText.text = TranslationService.Get ().Translate(info.Message);
        this.mText.maxVisibleCharacters = 0;
        // force mesh update
        this.mText.ForceMeshUpdate (forceTextReparsing: true);
        // set the timer
        this.mTimer = this.TextDisplayDelay;
        // enable everything inside us
        SetChildrenActive (true);
    }

    void QueueTextbox (string message, Vector3 worldPosition)
    {
        // disable movement for everything
        CombatEventManager.InvokeDisableMovement ();
        // queue the message
        this.mMessageQueue.Enqueue (
            new TextboxInformation ()
            {
                Message = message,
                WorldPosition = worldPosition
            }
        );
        
        // only dequeue a message if the textbox is not visible yet
        if (this.mDisplayingMessage == false)
            this.DequeueMessage ();
    }
}
