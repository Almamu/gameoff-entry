using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TutorialSceneController : MonoBehaviour
{
    public PlayableDirector Director;

    public AudioListener DisableWhenPlay;
    
    public GameObject Player;

    private bool mFirstStageDone = false;
    
    private void PauseDirector ()
    {
        this.Director.playableGraph.GetRootPlayable (0).SetSpeed (0);
    }

    private void ResumeDirector ()
    {
        this.Director.playableGraph.GetRootPlayable (0).SetSpeed (1);
    }
    
    void Start ()
    {
        // once a textbox is hidden resume playing
        CombatEventManager.TextboxHidden += this.ResumeDirector;
    }
    
    public void FirstMessage (Transform transform)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES1", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES2", MessageSource.Sarge);
    }

    public void FirstAnswer (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("FUCK.SARGE", MessageSource.Player);
    }

    public void SargeAnswer (Transform sarge)
    {
        CombatEventManager.InvokeTextbox ("FUCK.SARGE.ANSWER", MessageSource.Sarge);
    }

    public void SargeShoot (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SHOOT.ANSWER", MessageSource.Player);
    }

    public void FollowMe (Transform sarge)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SARGE.FOLLOWME", MessageSource.Sarge);
    }

    public void CutsceneEnd ()
    {
        this.DisableWhenPlay.enabled = false;
        this.Player.SetActive (true);
        
        // show text with the guide
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL1", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL2", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL3", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL4", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL5", MessageSource.Sarge);
    }

    public bool IsFirstStageDone ()
    {
        return this.mFirstStageDone;
    }

    public void SetFirstStageDone ()
    {
        this.mFirstStageDone = true;
    }
}
