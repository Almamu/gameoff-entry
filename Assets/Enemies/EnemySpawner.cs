using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// The amount of enemies that can be spawned at once
    /// </summary>
    public int MaximumActiveEnemies = 15;
    /// <summary>
    /// The total amount of enemies to go through
    /// </summary>
    public int AmountOfEnemies = 98;
    /// <summary>
    /// The total amount of active enemies right now
    /// </summary>
    public int ActiveEnemiesCount = 0;
    /// <summary>
    /// Probability for enemy b
    /// </summary>
    public float EnemyBProbability = 20.0f;
    
    /// <summary>
    /// Event fired when an enemy dies
    /// </summary>
    public static event Action<EnemySpawner> EnemyDeath;
    
    private GameObject [] mActiveEnemies;
    private NormalEnemyAObjectPool NormalEnemyAObjectPool;
    private NormalEnemyBObjectPool NormalEnemyBObjectPool;
    
    /// <summary>
    /// The list of spawn areas available for this spawner (fetched from the current component)
    /// </summary>
    private BoxCollider [] mSpawnAreas;
    /// <summary>
    /// The amount of enemies spawned
    /// </summary>
    private int mCurrentEnemies = 0;

    /// <summary>
    /// Checks the current spawn areas and picks up a random spawn point inside the boxes
    /// </summary>
    /// <returns></returns>
    private Vector3 PickupRandomSpawnPoint ()
    {
        BoxCollider area = this.mSpawnAreas [Random.Range (0, this.mSpawnAreas.Length)];
        
        return new Vector3 (
            Random.Range (area.bounds.min.x, area.bounds.max.x),
            this.transform.position.y,
            Random.Range (area.bounds.min.z, area.bounds.max.z)
        );
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.NormalEnemyAObjectPool = GetComponent <NormalEnemyAObjectPool>();
        this.NormalEnemyBObjectPool = GetComponent <NormalEnemyBObjectPool>();
        this.mActiveEnemies = new GameObject [this.MaximumActiveEnemies];
        Transform spawnArea = this.transform.Find ("SpawnArea");

        if (spawnArea is null)
            throw new InvalidDataException ("EnemySpawner must contain a child named SpawnArea");
        
        // spawn areas are inside this game object
        this.mSpawnAreas = this.transform.Find ("SpawnArea").GetComponents <BoxCollider> ();

        if (this.mSpawnAreas is null || this.mSpawnAreas.Length == 0)
            throw new InvalidDataException ("EnemySpawner must contain a child named SpawnArea with at least a box collider to mark the spawns");
        
        // create the required amount of enemies
        for (int i = 0; i < this.mActiveEnemies.Length && i <= this.AmountOfEnemies - this.mCurrentEnemies; i++)
            ActivateEnemy (i);
    }

    private void OnDestroy ()
    {
        EnemyDeath = null;
    }

    private void ActivateEnemy (int i)
    {
        // decrement counter
        this.mCurrentEnemies++;
        
        // randomly decide which enemy to activate
        float range = Random.Range (0.0f, 100.0f);
        float probability = this.EnemyBProbability * (math.lerp (0, 2, (float) this.mCurrentEnemies / (float) this.AmountOfEnemies));

        if (range < probability)
            this.mActiveEnemies [i] = this.NormalEnemyBObjectPool.Pop ();
        else
            this.mActiveEnemies [i] = this.NormalEnemyAObjectPool.Pop ();
        
        // take one enemy from the pool
        this.mActiveEnemies [i].transform.position = this.PickupRandomSpawnPoint ();
        this.mActiveEnemies [i].SetActive (true);
        this.ActiveEnemiesCount++;
    }

    void OnEnemyDead (GameObject enemy)
    {
        for (int i = 0; i < this.mActiveEnemies.Length; i ++)
        {
            if (enemy != this.mActiveEnemies [i])
                continue;

            this.ActiveEnemiesCount--;
            
            // fire the death events
            EnemyDeath?.Invoke (this);
            
            // move it back to the object pool
            this.mActiveEnemies [i] = null;
            
            // if required create a new one in place
            if (this.mCurrentEnemies >= this.AmountOfEnemies && this.ActiveEnemiesCount <= 1)
                break;

            this.ActivateEnemy (i);
        }
    }

    public int EnemiesLeft ()
    {
        return this.AmountOfEnemies + this.ActiveEnemiesCount - this.mCurrentEnemies;
    }
}
