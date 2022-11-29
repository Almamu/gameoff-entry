using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagStatusBehaviour : MonoBehaviour
{
    private Button mButton;
    private TranslationService mTranslation;

    public string ExpectedLanguage;
    
    // Start is called before the first frame update
    void Start()
    {
        this.mButton = this.GetComponent <Button> ();
        this.mTranslation = TranslationService.Get ();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.mTranslation.GetDefaultLanguage () == this.ExpectedLanguage)
            this.mButton.Select ();
    }
}
