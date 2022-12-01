using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class BossDeathSceneController : MonoBehaviour
{
    private PlayableDirector Director;
    
    void Start ()
    {
        this.Director = GetComponent <PlayableDirector> ();
        // once a textbox is hidden resume playing
        CombatEventManager.TextboxHidden += this.ResumeDirector;
    }
    
    private void PauseDirector ()
    {
        this.Director.playableGraph.GetRootPlayable (0).SetSpeed (0);
    }

    private void ResumeDirector ()
    {
        this.Director.playableGraph.GetRootPlayable (0).SetSpeed (1);
    }
    
    public void BossDeath ()
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("END.ALEXANDER", MessageSource.Other);
    }

    public void FinalTexts ()
    {
        this.PauseDirector ();

        CombatEventManager.InvokeTextbox ("END.SARGE1", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("END.SARGE2", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("END.SARGE3", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("END.SARGE4", MessageSource.Sarge);
    }

    public void GoToCredits ()
    {
        SceneManager.LoadScene ("Credits");
    }
}
