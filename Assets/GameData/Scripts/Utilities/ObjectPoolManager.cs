using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Enums;

namespace Match3.Utilities
{
    public class ObjectPoolManager : MonoBehaviour
    {
        #region SingleTon
        public static ObjectPoolManager instance = null;
        #endregion

        #region Inspector Variables
        [SerializeField] private Transform poolObjectsParent = null;
        [SerializeField] private List<PoolObject> poolObjects = null;
        #endregion

        #region Variables
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
                instance = this;
            }
            DontDestroyOnLoad(this);
            CreatePool();
        }
        #endregion

        #region Private Methods
        private void CreatePool()
        {
            if (poolObjects != null)
            {
                for (int i = 0; i < poolObjects.Count; i++)
                {
                    if (poolObjects[i].objectToPool != null)
                    {
                        poolObjects[i].pooledObjects = new List<GameObject>();
                        for (int j = 0; j < poolObjects[i].maxPoolCount; j++)
                        {
                            GameObject tempPoolObject = Instantiate(poolObjects[i].objectToPool, new Vector3(0f, 0f, 0f), Quaternion.identity);
                            tempPoolObject.SetActive(false);
                            poolObjects[i].pooledObjects.Add(tempPoolObject);
                            tempPoolObject.transform.SetParent(poolObjectsParent);
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public GameObject GetPoolObject(PoolObjectsType poolObjectsType)
        {
            if(poolObjects != null)
            {
                foreach (var item in poolObjects)
                {
                    if(item.poolObjectsType == poolObjectsType)
                    {
                        if(item.pooledObjects.Count > 0)
                        {
                            foreach(var effect in item.pooledObjects)
                            {
                                if (!effect.activeInHierarchy)
                                {
                                    return effect;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }

    [System.Serializable]
    public class PoolObject
    {
        public GameObject objectToPool;
        public int maxPoolCount = -1;
        public List<GameObject> pooledObjects = null;
        public PoolObjectsType poolObjectsType = PoolObjectsType.None;
    }
}