using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteractionHandlerBehaviour : MonoBehaviour
{
    public void OnSpanishClicked ()
    {
        TranslationService.Get ().SetDefaultLanguage ("ES");
    }

    public void OnEnglishClicked ()
    {
        TranslationService.Get ().SetDefaultLanguage ("EN");
    }
}
