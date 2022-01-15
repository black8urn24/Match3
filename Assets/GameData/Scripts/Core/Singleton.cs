using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region SingleTon
        private static T instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>();
                    if(instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        #endregion

        #region Unity Methods
        public virtual void Awake()
        {
            if(instance == null)
            {
                instance = this as T;
                transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}