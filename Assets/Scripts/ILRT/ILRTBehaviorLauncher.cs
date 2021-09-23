//#define __ENABLED_ILRT_DEBUG__                  // 是否开启 ILRuntime 的调试
#define __ILRT_DEBUGGER_AUTO_LISTENER__         // ILRuntime 是否自动使用 56000 开启断点监听
#define __WAITING_FOR_ATTACHING_ILRT_DEBUGGER__ // 是否 等待 hot fix 工程 attach 到 ILRuntime 调试器

// jave.lin 2021/09/17
// ILRuntime 层级的 Behavior 的执行器

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public static class ILRT_DEBUG_BRIDGE
{
    public static ILRuntime.Runtime.Enviorment.AppDomain domain
    {
        get; private set;
    }
    //public static void Create()
    //{
    //    domain = new ILRuntime.Runtime.Enviorment.AppDomain();
    //}
    public static void Dispose()
    {
        domain = null;
    }
    public static void Set(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        ILRT_DEBUG_BRIDGE.domain = domain;
    }
}

public class ILRTBehaviorLauncher : MonoSingleton<ILRTBehaviorLauncher>
{
    public static Action onHotFixLoaded;
    public static Action onUpdate;
    public static Action onLateUpdate;
    public static Action onFixedUpdate;
    public static Action<bool> onFocusChanged;
    public static Action onQuit;

    public ILRuntime.Runtime.Enviorment.AppDomain domain { get; private set; }
    public bool hotFixInited { get; private set; }

    private ILRT_HotFixLoader hotFixLoader;

    private void Awake()
    {
#if UNITY_EDITOR && DEBUG && __ENABLED_ILRT_DEBUG__
        domain = ILRT_DEBUG_BRIDGE.domain;
        if (domain == null)
        {
            domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        }
#else
        domain = new ILRuntime.Runtime.Enviorment.AppDomain();
#endif
        RegisterDelegates();
        StartLoadHotFix();
    }

    private void DestroyDomain()
    {
        if (domain != null)
        {
            // jave.lin : 清理 domain 时，记得要将 debugger 中也 stop 一下，分则可能有端口占用的问题
            if (domain.DebugService.IsStarted || domain.DebugService.IsDebuggerAttached)
            {
                domain.DebugService.StopDebugService();
            }
            domain = null;
        }
    }

    private void StartLoadHotFix()
    {
        Debug.Log($"ILRT Hot Fix Loader Start");
        hotFixLoader = new ILRT_HotFixLoader();
        hotFixLoader.onLoaded = onHotFixLoadedHandle;
        StartCoroutine(hotFixLoader.Start(domain));
    }

    private void RegisterDelegates()
    {
        //domain.DelegateManager.RegisterMethodDelegate<GameObject>();
        //domain.DelegateManager.RegisterMethodDelegate<ILRTComp>();
        domain.DelegateManager.RegisterMethodDelegate<bool>();
        domain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
    }

    private void onHotFixLoadedHandle()
    {
        Debug.Log($"{nameof(ILRTBehaviorLauncher)}.onHotFixLoadedHandle");
        onHotFixLoaded?.Invoke();

#if UNITY_EDITOR && DEBUG && __ENABLED_ILRT_DEBUG__ && __WAITING_FOR_ATTACHING_ILRT_DEBUGGER__
        StartCoroutine(WaitingAttachILRTDebuggerAndHotFixInit());
#else
#if __ILRT_DEBUGGER_AUTO_LISTENER__
        if (!domain.DebugService.IsStarted)
        {
            domain.DebugService.StartDebugService(56000);
        }
#endif
        onHotFixLauncherInit();
#endif
    }

    private IEnumerator WaitingAttachILRTDebuggerAndHotFixInit()
    {
        int times = 0;
#if UNITY_EDITOR
        var waitingViewPrefabPath = "Assets/UI/Prefabs/ShowTipsForWaitingAttachILRTDebugger.prefab";
        var waitingViewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(waitingViewPrefabPath);
        var waitingViewInst = GameObject.Instantiate(waitingViewPrefab);
        var parent = GameObject.Find("Canvas/Dialog");
        waitingViewInst.transform.SetParent(parent.transform);
        var rectTrans = waitingViewInst.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
        rectTrans.sizeDelta = Vector2.zero;
#endif
        if (!domain.DebugService.IsStarted)
        {
            domain.DebugService.StartDebugService(56000);
        }

        while (!domain.DebugService.IsDebuggerAttached)
        {
            Debug.Log($"Waiting For Attaching ILRuntime Debugger..., time : {++times}");
            yield return new WaitForSeconds(1.0f);
        }
        Debug.Log($"ILRuntime Debugger was Attached!, use Waiting Times : {times}");

#if UNITY_EDITOR
        GameObject.Destroy(waitingViewInst);
#endif
        onHotFixLauncherInit();
    }

    private void onHotFixLauncherInit()
    {
        //domain.Invoke("HotFix.HotFixLauncher", "Init", null, null);

        var ilType = domain.GetType("HotFix.HotFixLauncher");
        var ilMethod = ilType.GetMethod("Init", 0);
        using (var ctx = domain.BeginInvoke(ilMethod))
        {
            ctx.Invoke();
        }
        hotFixInited = true;
    }

    private void OnDestroy()
    {
        DestroyDomain();
    }

    private void Update()
    {
        if (hotFixInited)
        {
            ILRTCompReplaceHelper.HandlePendingList(domain);
        }
        onUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        onLateUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        onFocusChanged?.Invoke(focus);
    }

    private void OnApplicationQuit()
    {
        onQuit?.Invoke();
        if (hotFixLoader != null)
        {
            hotFixLoader.Dispose();
            hotFixLoader = null;
        }
        DestroyDomain();
    }

}
