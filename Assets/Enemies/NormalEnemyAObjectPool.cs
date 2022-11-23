using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyAObjectPool : MonoBehaviour
{
    private List<GameObject> mPooledObjects;

    /// <summary>
    /// The prefab to pool
    /// </summary>
    public GameObject PooledPrefabs;

    /// <summary>
    /// The amount of items to pool
    /// </summary>
    public int PoolLimit = 100;

    private GameObject[] mPool;
    
    void Awake()
    {
        this.mPool = new GameObject [PoolLimit];
        
        for (int i = 0; i < PoolLimit; i++)
        {
            GameObject obj = Instantiate(PooledPrefabs, gameObject.transform);
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
