// jave.lin 2021/09/23
// ILRuntime HotFix Project 用的 List<T> 的緩存池
// 不要直接使用 主工程的 ListPoolUtil<T>
// 原因可以参考：
// iOS IL2CPP打包注意事项
// https://ourpalm.github.io/ILRuntime/public/v1/guide/il2cpp.html
// 参考：泛型实例、泛型方法 两点的说明

using System.Collections.Generic;

public class ILRT_ListPoolUtil<T>
{
    private static int max = 500;
    public static int Max
    {
        get => max;
        set
        {
            if (max != value)
            {
                max = value;
                Trim();
            }
        }
    }

    private static Stack<List<T>> _list_pool = new Stack<List<T>>();
    public static List<T> FromPool()
    {
        return _list_pool.Count > 0 ? _list_pool.Pop() : new List<T>();
    }

    public static void ToPool(List<T> list)
    {
        list.Clear();
        _list_pool.Push(list);
    }

    public static void Clear()
    {
        _list_pool.Clear();
    }
    // trim stack<T>
    public static void Trim()
    {
        while (_list_pool.Count > 0 && _list_pool.Count > max)
        {
            _list_pool.Pop();
        }
        _list_pool.TrimExcess();
    }
    // trim stack<List<T>> 中的每个 List<T> 注意会有 GC
    public static void TrimList()
    {
        var arr = new List<T>[_list_pool.Count];
        _list_pool.CopyTo(arr, 0);
        for (int i = 0; i < arr.Length; i++)
        {
            var list = arr[i];
            list.TrimExcess();
        }
    }
}
