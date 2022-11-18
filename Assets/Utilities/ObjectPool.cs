using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> mPooledObjects;

    /// <summary>
    /// The prefab to pool
    /// </summary>
    public GameObject[] PooledPrefabs;

    /// <summary>
    /// The amount of items to pool
    /// </summary>
    public int PoolLimit = 500;

    private GameObject[] mPool;
    
    void Awake()
    {
        this.mPool = new GameObject [PoolLimit];

        int numberPrefabs = this.PooledPrefabs.Length;
        
        for (int i = 0; i < PoolLimit; i++)
        {
            GameObject randomPrefab = this.PooledPrefabs [Random.Range (0, numberPrefabs)];
            
            GameObject obj = Instantiate(randomPrefab, gameObject.transform);
            obj.SetActive(false);

            this.mPool[i] = obj;
        }
    }

    public GameObject Pop()
    {
        foreach (GameObject entry in this.mPool)
        {
            if (entry.activeInHierarchy == false)
                return entry;
        }

        return null;
    }
}
