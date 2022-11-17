using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Bosses
{
    public class RacimoObjectPool : MonoBehaviour
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
        
        void Awake()
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
                if (entry.activeInHierarchy == false && entry.transform.childCount > 0)
                    return entry;
            }

            return null;
        }
    }
}