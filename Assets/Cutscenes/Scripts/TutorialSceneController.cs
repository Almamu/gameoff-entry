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
        
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES1", true);
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES2", true);
    }

    public void FirstAnswer (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("FUCK.SARGE", false);
    }

    public void SargeAnswer (Transform sarge)
    {
        CombatEventManager.InvokeTextbox ("FUCK.SARGE.ANSWER", true);
    }

    public void SargeShoot (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SHOOT.ANSWER", false);
    }

    public void FollowMe (Transform sarge)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SARGE.FOLLOWME", true);
    }

    public void CutsceneEnd ()
    {
        this.DisableWhenPlay.enabled = false;
        this.Player.SetActive (true);
        
        // show text with the guide
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL1", true);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL2", true);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL3", true);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL4", true);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL5", true);
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
