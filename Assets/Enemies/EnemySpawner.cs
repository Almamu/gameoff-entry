using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    
    private GameObject [] mActiveEnemies;
    private ObjectPool mObjectPool;
    
    /// <summary>
    /// The list of spawn areas available for this spawner (fetched from the current component)
    /// </summary>
    private BoxCollider [] mSpawnAreas;

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
        this.mObjectPool = GetComponent <ObjectPool> ();
        this.mActiveEnemies = new GameObject [this.MaximumActiveEnemies];
        Transform spawnArea = this.transform.Find ("SpawnArea");

        if (spawnArea is null)
            throw new InvalidDataException ("EnemySpawner must contain a child named SpawnArea");
        
        // spawn areas are inside this game object
        this.mSpawnAreas = this.transform.Find ("SpawnArea").GetComponents <BoxCollider> ();

        if (this.mSpawnAreas is null || this.mSpawnAreas.Length == 0)
            throw new InvalidDataException ("EnemySpawner must contain a child named SpawnArea with at least a box collider to mark the spawns");
        
        // create the required amount of enemies
        for (int i = 0; i < this.mActiveEnemies.Length; i++)
        {
            this.AmountOfEnemies--;

            this.mActiveEnemies [i] = this.mObjectPool.Pop ();
            this.mActiveEnemies [i].SetActive (true);
            this.mActiveEnemies [i].transform.position = this.PickupRandomSpawnPoint ();
        }
    }

    void OnEnemyDead (GameObject enemy)
    {
        for (int i = 0; i < this.mActiveEnemies.Length; i ++)
        {
            if (enemy != this.mActiveEnemies [i])
                continue;

            // move it back to the object pool
            this.mActiveEnemies [i] = null;
            
            // decrement counter
            this.AmountOfEnemies--;

            // if required create a new one in place
            if (this.AmountOfEnemies <= 0)
                break;

            this.mActiveEnemies [i] = this.mObjectPool.Pop ();
            this.mActiveEnemies [i].SetActive (true);
            this.mActiveEnemies [i].transform.position = this.PickupRandomSpawnPoint ();
        }
    }
}
