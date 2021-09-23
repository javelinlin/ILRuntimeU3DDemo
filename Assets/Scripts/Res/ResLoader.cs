// jave.lin 2021/09/22
// 资源加载器

using System;
using UnityEngine;

public class ResLoader
{
    private static ResLoader _inst;
    public static ResLoader inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new ResLoader();
            }
            return _inst;
        }
    }
    public static void DisposeInst()
    {
        if (_inst != null)
        {
            _inst.Dispose();
            _inst = null;
        }
    }

    public Func<string, Type, UnityEngine.Object> loadHandle;

    public void Dispose()
    {
        loadHandle = null;
    }

    public T LoadSync<T>(string url) where T : UnityEngine.Object
    {
        if (loadHandle == null)
        {
            loadHandle = OnDefaultLoadHandle;
        }
        T ret = default(T);
        try
        {
            ret = loadHandle(url, typeof(T)) as T;
        }
        catch (Exception er)
        {
            Debug.LogError($"{nameof(ResLoader)}.LoadSync Error : {er}");
        }
        return ret;
    }

    private UnityEngine.Object OnDefaultLoadHandle(string path, Type type)
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
    }
}
