using System;
using UnityEngine;

namespace JiongXiaGu
{

    public class UnitySingleton<T> : MonoBehaviour
        where T : UnitySingleton<T>
    {
        protected static T instance;
        public static T Instance => instance;

        public static void Initialize()
        {
            GetOrCreate();
        }

        public static bool TryFind(out T value)
        {
            var values = FindObjectsOfType<T>();
            if (values.Length == 0)
            {
                value = default;
                return false;
            }
            else if (values.Length == 1)
            {
                value = values[0];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static T GetOrCreate()
        {
            if (instance != null)
                return instance;

            if (!TryFind(out instance))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    throw new InvalidOperationException("Not found instance in scene");
                }
#endif
                var item = new GameObject(typeof(T).Name);
                DontDestroyOnLoad(item);
                instance = item.AddComponent<T>();
            }
            return instance;
        }

        protected static void RemoveInstance(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (instance == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                instance = null;
            }
        }
    }
}
