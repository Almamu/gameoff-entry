using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextProTranslation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI textMesh = this.GetComponent <TextMeshProUGUI> ();
        textMesh.text = TranslationService.Get ().Translate (textMesh.text);
    }
}
