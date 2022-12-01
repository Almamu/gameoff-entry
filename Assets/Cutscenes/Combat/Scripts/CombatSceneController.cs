using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CombatSceneController : MonoBehaviour
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

    public void StartTextbox ()
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("CUTSCENE.WAKEUP", MessageSource.Player);
    }

    public void RadioConversation ()
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO1", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO2", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO3", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO4", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO5", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO6", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO7", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO8", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO9", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.RADIO10", MessageSource.Player);
    }

    public void MikeConversation ()
    {
        this.PauseDirector ();

        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE1", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE2", MessageSource.Other);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE3", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE4", MessageSource.Other);
    }

    public void MikeConversationSecond ()
    {
        this.PauseDirector ();
        
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE5", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE6", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE7", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE8", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE9", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE10", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE11", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.MIKE12", MessageSource.Sarge);
    }

    public void EagleAppeared ()
    {
        this.PauseDirector ();

        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE1", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE2", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE3", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE4", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE5", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE6", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE7", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE8", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE9", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE10", MessageSource.Sarge);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE11", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.EAGLE12", MessageSource.Sarge);
    }

    public void ScientificAppears ()
    {
        this.PauseDirector ();

        CombatEventManager.InvokeTextbox ("CUTSCENE.SCIENTIFIC1", MessageSource.Player);
        CombatEventManager.InvokeTextbox ("CUTSCENE.SCIENTIFIC2", MessageSource.Other);
        CombatEventManager.InvokeTextbox ("CUTSCENE.SCIENTIFIC3", MessageSource.Other);
    }

    public void GoToNextScene ()
    {
        CombatEventManager.ClearEvents ();
        // finally go to the playground
        SceneManager.LoadScene ("Playground");
    }
}
