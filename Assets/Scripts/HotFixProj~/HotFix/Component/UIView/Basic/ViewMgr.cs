// jave.lin 2021/09/22
// UI View 管理器

using Assets.Scripts.ILRT;
using HotFix.Mgr;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix.Component.UIView.Basic
{
    public struct ShowViewInfo
    {
        public string name;
        public ViewLayer layer;
        public Transform parent;
    }
    public class ViewMgr
    {
        private static ViewMgr _inst;
        public static ViewMgr inst 
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new ViewMgr();
                }
                return _inst;
            }
        }

        public Action<GameObject> ReplaceViewInstCallback;

        private Dictionary<string, UIViewBasic> instDict = new Dictionary<string, UIViewBasic>();

        public void Destroy()
        {
            var list = ILRT_ListPoolUtil<UIViewBasic>.FromPool();
            list.AddRange(instDict.Values);
            for (int i = 0; i < list.Count; i++)
            {
                var view = list[i];
                view.Dispose();
            }
            ILRT_ListPoolUtil<UIViewBasic>.ToPool(list);
            instDict.Clear();
        }
        public void AddView(string name, UIViewBasic view)
        {
            if (view == null)
            {
                ILRTDebug.LogError($"{nameof(ViewMgr)}.Add(name, view), name : {name}, view is null.");
                return;
            }

            if (!instDict.TryGetValue(name, out UIViewBasic srcView))
            {
                instDict[name] = view;
            }
            else
            {
                if (srcView != view)
                {
                    ILRTDebug.LogError($"{nameof(ViewMgr)}.Add(name, view) Repeat View Name, name : {name}");
                }
            }
        }
        public UIViewBasic GetView(string name)
        {
            instDict.TryGetValue(name, out UIViewBasic ret);
            return ret;
        }
        // jave.lin : 注意这个仅仅只是移除自 ViewMgr 中的 Dict 管理
        // 想要真正删除实例，请使用该 API：DisposeView
        public bool RemoveView(string name)
        {
            return instDict.Remove(name);
        }
        // 想要真正删除实例，请使用该 API
        public void DisposeView(string name, bool destroyOwner = true)
        {
            var view = GetView(name);
            RemoveView(name);
            if (view != null)
            {
                if (destroyOwner && view.Owner != null)
                {
                    GameObject.Destroy(view.Owner);
                    view.Owner = null;
                }
                view.Dispose();
            }
        }
        public UIViewBasic ShowView(ShowViewInfo showInfo)
        {
            return ShowView(showInfo.name, showInfo.layer, showInfo.parent);
        }
        public UIViewBasic ShowView(string name, ViewLayer layer = ViewLayer.Content, Transform parent = null)
        {
            var path = $"Assets/UI/Prefabs/{name}.prefab";
            var view = GetView(name);
            if (view == null)
            {
                var prefab = ResLoader.inst.LoadSync<GameObject>(path);
                var inst = GameObject.Instantiate(prefab);
                ReplaceViewInstCallback?.Invoke(inst);
                view = GetView(name);
            }
            if (view == null)
            {
                ILRTDebug.LogError($"{nameof(ViewMgr)}.ShowView(name), name : no instancing~");
                return null;
            }
            Transform useParent = ViewLayerGoMgr.GetUseParent(layer, parent);
            view.Show(useParent);
            return view;
        }
        public void HideView(string name)
        {
            var view = GetView(name);
            if (view == null)
            {
                ILRTDebug.LogError($"{nameof(ViewMgr)}.HideView(name), name : no instancing~");
                return;
            }
            view.Hide();
        }
        public void MoveToLayer(UIViewBasic view, ViewLayer layer, Transform parent)
        {
            Transform useParent = ViewLayerGoMgr.GetUseParent(layer, parent);
            view.Owner.transform.SetParent(useParent);
        }
        public void MoveToLayer(string name, ViewLayer layer, Transform parent)
        {
            var view = GetView(name);
            if (view == null)
            {
                ILRTDebug.LogError($"{nameof(ViewMgr)}.MoveToLayer(name), name : no instancing~");
                return;
            }
            MoveToLayer(view, layer, parent);
        }
        // jave.lin : 如果制作的好的话，还会有 Push, Pop 的 Stack<View> 的管理
        // 这样就可以实现返回上次界面的功能
        // 但是这里只是演示 ILRuntime 的简单使用，就不必写下去了
        public void PushView(string name)
        {
            // code here
            throw new NotImplementedException();
        }
        public void Pop()
        {
            throw new NotImplementedException();
        }
    }
}
