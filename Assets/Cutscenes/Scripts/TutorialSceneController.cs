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
        
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES1", transform.position);
        CombatEventManager.InvokeTextbox ("MOVE.ROOKIES2", transform.position);
    }

    public void FirstAnswer (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("FUCK.SARGE", protagonist.position);
    }

    public void SargeAnswer (Transform sarge)
    {
        CombatEventManager.InvokeTextbox ("FUCK.SARGE.ANSWER", sarge.position);
    }

    public void SargeShoot (Transform protagonist)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SHOOT.ANSWER", protagonist.position);
    }

    public void FollowMe (Transform sarge)
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("SARGE.FOLLOWME", sarge.position);
    }

    public void CutsceneEnd ()
    {
        this.DisableWhenPlay.enabled = false;
        this.Player.SetActive (true);
        
        // show text with the guide
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL1", Vector3.positiveInfinity);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL2", Vector3.positiveInfinity);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL3", Vector3.positiveInfinity);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL4", Vector3.positiveInfinity);
        CombatEventManager.InvokeTextbox ("SARGE.TUTORIAL5", Vector3.positiveInfinity);
    }
}
