using UnityEngine;

namespace CDA.Framework.Interface
{
    public class MonoSingleton<T> : MonoBehaviour  where T : MonoSingleton<T>
    {
        private static T _instance;
        private SingletonInitializationStatus initStatus = SingletonInitializationStatus.None;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        public virtual bool IsInitialized => this.initStatus == SingletonInitializationStatus.Initialized;
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                InitializeSingleton();
            }
            else
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
            }
        }
        
        protected virtual void OnInitialized() { }

        protected virtual void OnInitializing() { }

        public virtual void InitializeSingleton()
        {
            if (this.initStatus == SingletonInitializationStatus.None)
            {
                return;
            }

            this.initStatus = SingletonInitializationStatus.Initializing;
            OnInitializing();
            this.initStatus = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }
        
        public virtual void ClearSingleton() { }
        public static void CreateInstance()
        {
            DestroyInstance();
            _instance = Instance;
        }

        public static void DestroyInstance()
        {
            if (_instance == null)
                return;

            _instance.ClearSingleton();
            _instance = null;
        }
    }
}