namespace CDA.Framework.Interface
{
    public enum SingletonInitializationStatus
    {
        None,
        Initializing,
        Initialized
    }
    
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;
        private SingletonInitializationStatus _status = SingletonInitializationStatus.None;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(T))
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            _instance.InitializeSingleton();
                        }
                    }
                }
                return _instance;
            }
        }
        
        public bool IsInitialized
        {
            get { return _status == SingletonInitializationStatus.Initialized; }
        }

        protected virtual void OnInitializing(){}
        protected virtual void OnInitialized(){}

        public virtual void InitializeSingleton()
        {
            if (this._status != SingletonInitializationStatus.None)
                return;
            this._status = SingletonInitializationStatus.Initializing;
            OnInitializing();
            this._status = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }
        
        public virtual void ClearSingleton(){}

        public static void CreateInstance()
        {
            if(_instance != null) DestoryInstance();
            _instance = Instance;
        }

        public static void DestoryInstance()
        {
            if (_instance == null)
                return;
            _instance.ClearSingleton();
            _instance = null;
        }
    }
}