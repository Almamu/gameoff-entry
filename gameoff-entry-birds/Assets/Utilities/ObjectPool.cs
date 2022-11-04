using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> mPooledObjects;

    /// <summary>
    /// The prefab to pool
    /// </summary>
    public GameObject PooledPrefab;

    /// <summary>
    /// The amount of items to pool
    /// </summary>
    public int PoolLimit = 500;

    private GameObject[] mPool;
    
    void Start()
    {
        this.mPool = new GameObject [PoolLimit];
        
        for (int i = 0; i < PoolLimit; i++)
        {
            GameObject obj = Instantiate(PooledPrefab, gameObject.transform);
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
