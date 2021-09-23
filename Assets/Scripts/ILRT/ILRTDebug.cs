//#define __ILRT_LOG__

// jave.lin 2021/09/17
// ILRuntime 的 Debug 类

using UnityEngine;

namespace Assets.Scripts.ILRT
{
    public static class ILRTDebug
    {
        // jave.lin : 设置inline
        public static void Log(string msg)
        {
#if __ILRT_LOG__
        Debug.Log(msg);
#endif
        }
        public static void LogWarning(string msg)
        {
#if __ILRT_LOG__
        Debug.LogWarning(msg);
#endif
        }
        public static void LogError(string msg)
        {
#if __ILRT_LOG__
        Debug.LogError(msg);
#endif
        }
    }
}
