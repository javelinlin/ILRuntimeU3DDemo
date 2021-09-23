// jave.lin 2021/09/23
// List<T> 的緩存池

using System.Collections.Generic;

public class ListPoolUtil<T>
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
