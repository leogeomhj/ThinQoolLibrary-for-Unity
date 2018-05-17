using System;
using System.Collections.Generic;

namespace TQL.Utill.Singleton
{
    public abstract class SingletonBase
    {
        public abstract void DestroySingleton();
    }

    public class Singleton<T> : SingletonBase where T : SingletonBase, new()
    {
        private static object syncRoot = new object();
        private static volatile T instance = null;
        public static T Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        Type type = typeof(T);

                        SingletonBase singleton = SingletonManager.Get(type);
                        if (singleton == null)
                        {
                            instance = new T();

                            SingletonManager.Add(type, instance);
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

        public override void DestroySingleton()
        {
            SingletonManager.Remove(typeof(T));

            instance = null;
        }
    }

    public static class SingletonManager
    {
        private static Dictionary<Type, SingletonBase> _singletons = new Dictionary<Type, SingletonBase>();

        internal static void Add(Type type, SingletonBase singleton)
        {
            if (_singletons.ContainsKey(type) == false)
            {
                _singletons.Add(type, singleton);
            }
        }

        internal static SingletonBase Get(Type type)
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
            Remove(type);
        }

        public static void DestroyAll()
        {
            List<SingletonBase> list = new List<SingletonBase>(_singletons.Values);

            var etor = list.GetEnumerator();
            while (etor.MoveNext())
            {
                etor.Current.DestroySingleton();
            }
            _singletons.Clear();
        }
    }
}
