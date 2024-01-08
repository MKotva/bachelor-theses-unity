using UnityEngine;

namespace Assets.Scripts.GameEditor
{
    //TODO: Reference properly!! This code was found on Gitlab: https://gitlab.com/trampod/gamejam-skeleton/-/blob/master/Assets/Scripts/Utils/Singleton.cs?ref_type=heads

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region  Fields
        private static T _instance;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        [SerializeField]
        private bool _persistent = false;
        #endregion

        #region  Properties
        public static bool Quitting { get; private set; }

        /// <summary>
        /// Singleton instance of <see cref="T"/>. If such instance doesn't exist it will create new one.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning($"[{nameof(Singleton<T>)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }
                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;

                    return SetInstance();
                }
            }
        }
        #endregion

        #region  Methods
        private static T SetInstance()
        {
            var instances = FindObjectsOfType<T>();
            var count = instances.Length;
            if (count > 0)
            {
                if (count == 1)
                    return _instance = instances[0];
                Debug.LogWarning($"[{nameof(Singleton<T>)}<{typeof(T)}>] There should never be more than one {nameof(Singleton<T>)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                for (var i = 1; i < instances.Length; i++)
                    Destroy(instances[i]);
                return _instance = instances[0];
            }

            Debug.Log($"[{nameof(Singleton<T>)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
            return _instance = new GameObject($"({nameof(Singleton<T>)}){typeof(T)}")
                       .AddComponent<T>();
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                SetInstance();
            }

            if (_persistent)
                DontDestroyOnLoad(gameObject);

        }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
        #endregion
    }
}
