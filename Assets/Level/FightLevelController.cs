using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightLevelController : MonoBehaviour
{
    /// <summary>
    /// The boss controller
    /// </summary>
    private BossesController BossesController;
    
    void Start ()
    {
        this.BossesController = this.GetComponentInChildren <BossesController> (true);
        
        EnemySpawner.EnemyDeath += this.OnEnemyDeath;
    }

    private void OnEnemyDeath (EnemySpawner spawner)
    {
        // do not do anything if there's any enemy active
        if (spawner.ActiveEnemiesCount > 0)
            return;
        
        // TODO: KEEP TRACK OF STATUS
        
        // activate the boss and disable the enemy spawner
        spawner.gameObject.SetActive (false);
        this.BossesController.gameObject.SetActive (true);
    }
}
