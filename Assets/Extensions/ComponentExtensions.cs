using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class ComponentExtensions 
    {
        public static T GetComponentOnlyInChildren <T> (this MonoBehaviour behaviour) where T : class
        {
            if (typeof(T).IsInterface == false &&
                typeof(T).IsSubclassOf (typeof(Component)) == false &&
                typeof(T) != typeof(Component))
                return null;
        
            foreach (Transform transform in behaviour.transform)
            {
                T result = transform.GetComponent <T> ();

                if (result is not null)
                    return result;
            }

            return null;
        }

        public static Vector3 GetRandomPositionInside (this Bounds bounds)
        {
            return new Vector3 (
                Random.Range (bounds.min.x, bounds.max.x),
                Random.Range (bounds.min.y, bounds.max.y),
                Random.Range (bounds.min.z, bounds.max.z)
            );
        }
    }

}
