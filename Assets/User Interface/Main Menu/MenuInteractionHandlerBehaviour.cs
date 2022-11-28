using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void OnStartClicked ()
    {
        SceneManager.LoadScene ("Training");
    }

    public void OnExitClicked ()
    {
        Application.Quit();
    }
}
