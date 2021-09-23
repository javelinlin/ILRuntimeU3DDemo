// jave.lin 2021/09/22
// MonoSingle 单例

using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst = null;
    public static T inst
    {
        get
        {
            if (_inst == null)
            {
                Type type = typeof(T);
                _inst = (T)FindObjectOfType(type);
                if (_inst == null)
                {
                    GameObject go = new GameObject(type.ToString());
                    DontDestroyOnLoad(go);
                    _inst = go.AddComponent<T>();
                }
            }
            return _inst;
        }
    }
}