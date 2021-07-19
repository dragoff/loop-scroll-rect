/* (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com */


using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public string Name;

        //defines whether the object is waiting in pool or is in use
        public bool IsPooled;
    }

    [Serializable]
    public class PoolData
    {
        public string Name => prefab != null ? prefab.name : name;
        public GameObject Prefab => prefab;

        private string name;
        private GameObject prefab;

        public PoolData(string name, GameObject prefab)
        {
            this.name = name;
            this.prefab = prefab;
        }
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, add one to the pool.
        Increment,

        /// When a dynamic pool inflates, double the size of the pool
        Double
    }

    class Pool
    {
        private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

        //the root obj for unused obj
        private GameObject rootObj;
        private PoolInflationType inflationType;
        private string poolName;
        private int objectsInUse = 0;

        public Pool(string poolName, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount,
            PoolInflationType type)
        {
            if (poolObjectPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[Loop Scroll Rect] null pool object prefab !");
#endif
                return;
            }

            this.poolName = poolName;
            this.inflationType = type;
            this.rootObj = new GameObject(poolName + "Pool");
            this.rootObj.transform.SetParent(rootPoolObj.transform, false);

            // In case the origin one is Destroyed, we should keep at least one
            GameObject go = Object.Instantiate(poolObjectPrefab);
            var po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                po = go.AddComponent<PoolObject>();
            }

            po.Name = poolName;
            AddObjectToPool(po);

            //populate the pool
            populatePool(Mathf.Max(initialCount, 1));
        }

        //o(1)
        private void AddObjectToPool(PoolObject po)
        {
            //add to pool
            po.gameObject.SetActive(false);
            po.gameObject.name = poolName;
            availableObjStack.Push(po);
            po.IsPooled = true;
            //add to a root obj
            po.gameObject.transform.SetParent(rootObj.transform, false);
        }

        private void populatePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                var po = GameObject.Instantiate(availableObjStack.Peek());
                AddObjectToPool(po);
            }
        }

        //o(1)
        public GameObject NextAvailableObject(bool autoActive)
        {
            PoolObject po = null;
            if (availableObjStack.Count > 1)
            {
                po = availableObjStack.Pop();
            }
            else
            {
                int increaseSize = 0;
                //increment size var, this is for info purpose only
                if (inflationType == PoolInflationType.Increment)
                {
                    increaseSize = 1;
                }
                else if (inflationType == PoolInflationType.Double)
                {
                    increaseSize = availableObjStack.Count + Mathf.Max(objectsInUse, 0);
                }
#if UNITY_EDITOR
                Debug.Log(string.Format("[Loop Scroll Rect] Growing pool {0}: {1} populated", poolName, increaseSize));
#endif
                if (increaseSize > 0)
                {
                    populatePool(increaseSize);
                    po = availableObjStack.Pop();
                }
            }

            GameObject result = null;
            if (po != null)
            {
                objectsInUse++;
                po.IsPooled = false;
                result = po.gameObject;
                if (autoActive)
                {
                    result.SetActive(true);
                }
            }

            return result;
        }

        //o(1)
        public void ReturnObjectToPool(PoolObject po)
        {
            if (poolName.Equals(po.Name))
            {
                objectsInUse--;
                /* we could have used availableObjStack.Contains(po) to check if this object is in pool.
                 * While that would have been more robust, it would have made this method O(n) 
                 */
                if (po.IsPooled)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("[Loop Scroll Rect]" + po.gameObject.name +
                                     " is already in pool. Why are you trying to return it again? Check usage.");
#endif
                }
                else
                {
                    AddObjectToPool(po);
                }
            }
            else
            {
                Debug.LogError(string.Format("[Loop Scroll Rect] Trying to add object to incorrect pool {0} {1}",
                    po.Name, poolName));
            }
        }
    }
}