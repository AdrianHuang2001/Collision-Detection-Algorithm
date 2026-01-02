using UnityEngine;

namespace CDA.Framework.Interface
{
    public class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {
        [SerializeField] private bool UnparentOnAwake = true;
        
        protected override void OnInitialized()
        {
            if(UnparentOnAwake) transform.SetParent(null);
            base.OnInitialized();
            if(Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }
    }
}