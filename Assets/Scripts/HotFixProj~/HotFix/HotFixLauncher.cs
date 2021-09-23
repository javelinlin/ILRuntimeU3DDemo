// jave.lin 2021/09/17
// HotFix 的

using Assets.Scripts.ILRT;
using HotFix.Behavior;
using HotFix.Component.UIView.Basic;
using HotFix.Mgr;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    public static class HotFixLauncher
    {
        private static List<IBehavior> behaviorList = new List<IBehavior>();
        private static List<IComp> compList = new List<IComp>();

        public static void Init()
        {
            ILRTDebug.Log($"{nameof(HotFixLauncher)}.Init() Start");

            ILRTBehaviorLauncher.onUpdate       = Update;
            ILRTBehaviorLauncher.onLateUpdate   = LateUpdate;
            ILRTBehaviorLauncher.onFixedUpdate  = FixedUpdate;
            ILRTBehaviorLauncher.onFocusChanged = OnApplicationFocus;
            ILRTBehaviorLauncher.onQuit         = OnApplicationQuit;

            ViewMgr.inst.ReplaceViewInstCallback = OnReplaceViewInstHandler;

            ViewLayerGoMgr.content    = GameObject.Find("Canvas/Content");
            ViewLayerGoMgr.dialog     = GameObject.Find("Canvas/Dialog");
            ViewLayerGoMgr.tips       = GameObject.Find("Canvas/Tips");

            AddMultiBehaviors();
            AddMultiComp();

            ILRTDebug.Log($"{nameof(HotFixLauncher)}.Init() End");
        }

        public static void ReplaceComp(ILRTComp comp)
        {
            // get and check type
            var type = Type.GetType(comp.ILRT_comp_name);
            if (type == null)
            {
                ILRTDebug.LogError($"{nameof(HotFixLauncher)}.AttachComp comp type name : {comp.ILRT_comp_name} is not exsits!");
                return;
            }
            // create
            var ilrt_comp_inst = Activator.CreateInstance(type) as IComp;
            if (ilrt_comp_inst == null)
            {
                ILRTDebug.LogError($"{nameof(HotFixLauncher)}.AttachComp comp type name : {comp.ILRT_comp_name} can not create!");
                return;
            }
            // set owner
            ilrt_comp_inst.Owner = comp.gameObject;
            // init
            ilrt_comp_inst.Init();
            ILRTDebug.Log($"{nameof(HotFixLauncher)}.AttachComp comp type name : {comp.ILRT_comp_name} create SUCCESSFUL!");
            // add to list
            AddSingleComp(ilrt_comp_inst);

            if (
                ilrt_comp_inst.GetType().IsSubclassOf(typeof(UIViewBasic))
                //comp.ILRT_comp_name.StartsWith("HotFix.Component.UIView") // 最好使用 IsSubclassOf 方式
                )
            {
                var idx = comp.ILRT_comp_name.LastIndexOf(".");
                var viewName = idx == -1 ? comp.ILRT_comp_name : comp.ILRT_comp_name.Substring(idx + 1);

                ILRTDebug.Log($"{nameof(HotFixLauncher)}.AttachComp, ViewMgr.inst.AddView(viewName) : viewName : {viewName}");
                // 添加到 view 管理
                ViewMgr.inst.AddView(viewName, ilrt_comp_inst as UIViewBasic);
            }
        }
        internal static void InternalAllReplaceComp(GameObject go)
        {
            if (go == null)
            {
                ILRTDebug.LogError($"{nameof(HotFixLauncher)}.InternalAllReplaceComp go == null.");
                return;
            }
            var list = ILRT_ListPoolUtil<ILRTComp>.FromPool();
            //var list = ListPoolUtil<ILRTComp>.FromPool(); // 这里尽量不要使用 主工程定义的泛型，原因可以查看 ILRT_ListPoolUtil<T> 中的注释
            go.GetComponentsInChildren<ILRTComp>(true, list);
            for (int i = 0; i < list.Count; i++)
            {
                var comp = list[i];
                if (comp.HadReplaced)
                {
                    continue;
                }
                ReplaceComp(comp);
            }
            ILRT_ListPoolUtil<ILRTComp>.ToPool(list);
            //ListPoolUtil<ILRTComp>.ToPool(list);
        }
        private static void Update()
        {
            // ILRuntime 的 Bug，没法在 Watch 面板中显示对应的 VariableInfo
            // 所以我们只能将 类全局成员下的对应 重置指向在 StackFrame 的 Scope 中的临时变量
            // 这样 ILRuntime Debugger 才能在 VS 的 Watch 面板下显示正常
            var tempBehaviorList = behaviorList;
            var tempCompList = compList;
            for (int i = tempBehaviorList.Count - 1; i > -1; i--)
            {
                var b = tempBehaviorList[i];
                if (b.IsDisposed)
                {
                    tempBehaviorList.RemoveAt(i);
                    continue;
                }
                if (!b.IsStarted)
                {
                    b.Start();
                }
                b.Update();
            }
            for (int i = tempCompList.Count - 1; i > -1; i--)
            {
                var c = tempCompList[i];
                if (c.IsDisposed || c.Owner == null)
                {
                    tempCompList.RemoveAt(i);
                    continue;
                }
                if (!c.IsStarted)
                {
                    c.Start();
                }
                c.Update();
            }
            ILRTDebug.Log($"HotFix.HotFixLauncher.Update()");
        }
        private static void LateUpdate()
        {
            // ILRuntime 的 Bug，没法在 Watch 面板中显示对应的 VariableInfo
            // 所以我们只能将 类全局成员下的对应 重置指向在 StackFrame 的 Scope 中的临时变量
            // 这样 ILRuntime Debugger 才能在 VS 的 Watch 面板下显示正常
            var tempBehaviorList = behaviorList;
            var tempCompList = compList;
            for (int i = tempBehaviorList.Count - 1; i > -1; i--)
            {
                var b = tempBehaviorList[i];
                if (b.IsDisposed)
                {
                    tempBehaviorList.RemoveAt(i);
                    continue;
                }
                b.LateUpdate();
            }
            for (int i = tempCompList.Count - 1; i > -1; i--)
            {
                var c = tempCompList[i];
                if (c.IsDisposed || c.Owner == null)
                {
                    tempCompList.RemoveAt(i);
                    continue;
                }
                c.LateUpdate();
            }
            ILRTDebug.Log($"HotFix.HotFixLauncher.LateUpdate()");
        }
        private static void FixedUpdate()
        {
            // ILRuntime 的 Bug，没法在 Watch 面板中显示对应的 VariableInfo
            // 所以我们只能将 类全局成员下的对应 重置指向在 StackFrame 的 Scope 中的临时变量
            // 这样 ILRuntime Debugger 才能在 VS 的 Watch 面板下显示正常
            var tempBehaviorList = behaviorList;
            var tempCompList = compList;
            for (int i = tempBehaviorList.Count - 1; i > -1; i--)
            {
                var b = tempBehaviorList[i];
                if (b.IsDisposed)
                {
                    tempBehaviorList.RemoveAt(i);
                    continue;
                }
                b.FixedUpdate();
            }
            for (int i = tempCompList.Count - 1; i > -1; i--)
            {
                var c = tempCompList[i];
                if (c.IsDisposed || c.Owner == null)
                {
                    tempCompList.RemoveAt(i);
                    continue;
                }
                c.FixedUpdate();
            }
            ILRTDebug.Log($"HotFix.HotFixLauncher.FixedUpdate()");
        }
        private static void OnApplicationFocus(bool focus)
        {
            // ILRuntime 的 Bug，没法在 Watch 面板中显示对应的 VariableInfo
            // 所以我们只能将 类全局成员下的对应 重置指向在 StackFrame 的 Scope 中的临时变量
            // 这样 ILRuntime Debugger 才能在 VS 的 Watch 面板下显示正常
            var tempBehaviorList = behaviorList;
            var tempCompList = compList;
            for (int i = tempBehaviorList.Count - 1; i > -1; i--)
            {
                var b = tempBehaviorList[i];
                if (b.IsDisposed)
                {
                    tempBehaviorList.RemoveAt(i);
                    continue;
                }
                b.OnApplicationFocus(focus);
            }
            for (int i = tempCompList.Count - 1; i > -1; i--)
            {
                var c = tempCompList[i];
                if (c.IsDisposed || c.Owner == null)
                {
                    tempCompList.RemoveAt(i);
                    continue;
                }
                c.OnApplicationFocus(focus);
            }
            ILRTDebug.Log($"HotFix.HotFixLauncher.OnApplicationFocus(bool focus), focus : {focus}");
        }
        private static void OnApplicationQuit()
        {
            // ILRuntime 的 Bug，没法在 Watch 面板中显示对应的 VariableInfo
            // 所以我们只能将 类全局成员下的对应 重置指向在 StackFrame 的 Scope 中的临时变量
            // 这样 ILRuntime Debugger 才能在 VS 的 Watch 面板下显示正常
            var tempBehaviorList = behaviorList;
            var tempCompList = compList;
            for (int i = tempBehaviorList.Count - 1; i > -1; i--)
            {
                var b = tempBehaviorList[i];
                if (b.IsDisposed)
                {
                    tempBehaviorList.RemoveAt(i);
                    continue;
                }
                b.OnApplicationQuit();
            }
            for (int i = tempCompList.Count - 1; i > -1; i--)
            {
                var c = tempCompList[i];
                if (c.IsDisposed || c.Owner == null)
                {
                    tempCompList.RemoveAt(i);
                    continue;
                }
                c.OnApplicationQuit();
            }
            ILRTDebug.Log($"HotFix.HotFixLauncher.OnApplicationQuit()");
        }

        private static void OnReplaceViewInstHandler(GameObject obj)
        {
            InternalAllReplaceComp(obj);
        }

        private static void AddMultiBehaviors()
        {
            AddSingleBehavior(BattleMgr.instance);
            AddSingleBehavior(CharacterMgr.instance);
            AddSingleBehavior(ItemMgr.instance);
        }

        private static void AddSingleBehavior(IBehavior b)
        {
            behaviorList.Add(b);
            ILRTDebug.Log($"Added behavior : {b.GetType().Name}");
        }

        private static void AddMultiComp()
        {
            
        }

        private static void AddSingleComp(IComp comp)
        {
            compList.Add(comp);
            ILRTDebug.Log($"Added component : {comp.GetType().Name}");
        }
    }
}
