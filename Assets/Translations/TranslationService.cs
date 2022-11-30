using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TranslationService
{
    private Dictionary <string, Dictionary <string, string>> mTranslations = new Dictionary <string, Dictionary <string, string>> ();
    private static TranslationService sInstance = null;
    private string DefaultLanguage = "EN";
    
    public TranslationService ()
    {
        TextAsset languagesFile = Resources.Load ("Translations/Languages") as TextAsset;

        string content = languagesFile.ToString ();
        string [] languages = content.Split ('\n');

        foreach (string languageLine in languages)
        {
            string language = languageLine.Trim ();
            Dictionary <string, string> data = new Dictionary <string, string> ();
            TextAsset languageFile = Resources.Load ($"Translations/{language.ToUpperInvariant ()}") as TextAsset;
            string [] lines = languageFile.ToString ().Split ('\n');

            foreach (string line in lines)
            {
                int separatorIndex = line.IndexOf (';');
                
                if (separatorIndex == -1)
                {
                    Debug.LogWarning ("Ignoring translation line as no separator was found");
                    continue;
                }

                string key = line.Substring (0, separatorIndex);
                string value = line.Substring (separatorIndex + 1).Replace ("\\n", "\n");

                data [key.ToUpperInvariant ()] = value;
            }

            this.mTranslations [language.ToUpperInvariant ()] = data;
        }
    }

    public static TranslationService Get ()
    {
        return sInstance ??= new TranslationService ();
    }

    public string Translate (string key)
    {
        if (this.mTranslations.TryGetValue (this.DefaultLanguage, out Dictionary <string, string> translations) == true &&
            translations.TryGetValue (key, out string value) == true)
            return value;

        Debug.LogWarning ($"Trying to translate {this.DefaultLanguage}::{key}, not found");
        
        return "MISSING TRANSLATION";
    }

    public void SetDefaultLanguage (string newLanguage)
    {
        if (this.mTranslations.ContainsKey (newLanguage) == false)
        {
            Debug.LogWarning ($"Trying to set default language not possible, {newLanguage} not found");
            return;
        }

        this.DefaultLanguage = newLanguage;
    }

    public string GetDefaultLanguage ()
    {
        return this.DefaultLanguage;
    }
}
