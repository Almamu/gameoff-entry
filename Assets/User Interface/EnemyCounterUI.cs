using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCounterUI : MonoBehaviour
{
    private TextMeshProUGUI mTextMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        this.mTextMesh = GetComponent <TextMeshProUGUI> ();
        EnemySpawner.EnemyDeath += this.EnemyDeath;
        this.mTextMesh.text = TranslationService.Get ().Translate ("ENEMIES.LEFT") + " 98";
    }

    void EnemyDeath (EnemySpawner source)
    {
        this.mTextMesh.text = TranslationService.Get ().Translate ("ENEMIES.LEFT") + " " + source.EnemiesLeft ().ToString ();
    }
}
