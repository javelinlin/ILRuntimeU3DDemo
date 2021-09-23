#define __REMOVE_REPLACE_COMP__ // 是否需要删除本来的组件

// jave.lin 2021/09/22
// ILRT 组件替换的助手类

using System.Collections.Generic;
using UnityEngine;

public static class ILRTCompReplaceHelper
{
    private static Queue<ILRTComp> pendingList = new Queue<ILRTComp>();

#if __REMOVE_REPLACE_COMP__
    public readonly static bool AFTER_REPLACE_REMOVE_COMP = true;
#else
    public readonly static bool AFTER_REPLACE_REMOVE_COMP = false;
#endif

    public static void Replace(ILRTComp comp, ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        var go = comp.gameObject;
        var ilType = domain.GetType("HotFix.HotFixLauncher");
        var ilMethod = ilType.GetMethod("ReplaceComp", 1);
        using (var ctx = domain.BeginInvoke(ilMethod))
        {
            ctx.PushObject(comp);
            ctx.Invoke();
        }

#if __REMOVE_REPLACE_COMP__
        GameObject.Destroy(comp);
#endif
    }
    public static void PushToPendingList(ILRTComp comp)
    {
        Debug.Log($"Push to pending list, comp : {comp.ILRT_comp_name}");
        pendingList.Enqueue(comp);
    }
    public static void HandlePendingList(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        while (pendingList.Count > 0)
        {
            var item = pendingList.Dequeue();
            Debug.Log($"Handle pending list, comp : {item.ILRT_comp_name}");
            Replace(item, domain);
        }
    }
}

public class PendingCompReplaceMission
{
    public ILRTComp comp;
}