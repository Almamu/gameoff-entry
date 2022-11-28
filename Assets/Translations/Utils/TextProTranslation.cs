using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextProTranslation : MonoBehaviour
{
    private TextMeshProUGUI mTextMesh;
    private string mTextKey;
    
    // Start is called before the first frame update
    void Start()
    {
        this.mTextMesh = this.GetComponent <TextMeshProUGUI> ();
        this.mTextKey = this.mTextMesh.text;
    }

    private void LateUpdate ()
    {
        this.mTextMesh.text = TranslationService.Get ().Translate (this.mTextKey);
    }
}
