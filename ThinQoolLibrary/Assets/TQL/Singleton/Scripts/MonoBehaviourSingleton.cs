using System;
using System.Collections.Generic;
using UnityEngine;

namespace TQL.Singleton
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object syncRoot = new object();
        private static volatile T instance = null;
        public static T Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (MonoBehaviourSingletonManager.IsQuitting)
                    {
                        return null;
                    }

                    if (instance == null)
                    {
                        Type type = typeof(T);

                        GameObject go = MonoBehaviourSingletonManager.Get(type);
                        if (go == null)
                        {
                            go = new GameObject();

                            MonoBehaviourSingletonManager.Add(type, go);

                            go.AddComponent<T>();
                        }
                        else
                        {
                            instance = go.GetComponent<T>();
                        }
                    }

                    return instance;
                }
            }
        }

        public static bool ValidInstance
        {
            get
            {
                return (instance != null);
            }
        }

        private bool isDestroySingleton = true;

        private void Awake()
        {
            useGUILayout = false;

            if (instance == null)
            {
                instance = this as T;

                InitializeSingleton();
            }
            else
            {
                isDestroySingleton = false;

                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (isDestroySingleton)
            {
                MonoBehaviourSingletonManager.Remove(typeof(T));

                instance = null;
            }
        }

#if !UNITY_2018_1_OR_NEWER
        protected virtual void OnApplicationQuit()
        {
            MonoBehaviourSingletonManager.OnQuitting();
        }
#endif

        protected virtual void InitializeSingleton()
        {
            GameObject cacheGameObject = gameObject;

            cacheGameObject.name = "(singleton)" + typeof(T).Name;

            DontDestroyOnLoad(cacheGameObject);
        }

        public virtual void Initialize()
        {
        }
    }

    public static class MonoBehaviourSingletonManager
    {
        private static Dictionary<Type, GameObject> _singletons = new Dictionary<Type, GameObject>();
        private static List<Type> _dontDestroySingletons = new List<Type>();

        internal static void Add(Type type, GameObject gameObject)
        {
            if (_singletons.ContainsKey(type) == false)
            {
                _singletons.Add(type, gameObject);
            }
        }

        internal static GameObject Get(Type type)
        {
            if (_singletons.ContainsKey(type) == true)
            {
                return _singletons[type];
            }

            return null;
        }

        internal static void Remove(Type type)
        {
            if (_singletons.ContainsKey(type) == true)
            {
                _singletons.Remove(type);
            }
        }

        public static void DontDestroySingleton(Type type)
        {
            if (_dontDestroySingletons.Contains(type) == false)
            {
                _dontDestroySingletons.Add(type);
            }
        }

        public static void DestroyAll()
        {
            List<Type> list = new List<Type>(_singletons.Keys);

            var etor = list.GetEnumerator();
            while (etor.MoveNext())
            {
                if (_dontDestroySingletons.Contains(etor.Current) == false)
                {
                    GameObject.Destroy(_singletons[etor.Current]);
                    _singletons.Remove(etor.Current);
                }
                else
                {
                    MonoBehaviour singleton = _singletons[etor.Current].GetComponent(etor.Current) as MonoBehaviour;
                    singleton.Invoke("Initialize", 0f);
                }
            }
        }

        public static bool IsQuitting { get; private set; }

#if !UNITY_2018_1_OR_NEWER
        public static void OnQuitting()
        {
            IsQuitting = true;
        }
#else
        private static void OnQuitting()
        {
            IsQuitting = true;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += OnQuitting;
        }
#endif
    }
}
