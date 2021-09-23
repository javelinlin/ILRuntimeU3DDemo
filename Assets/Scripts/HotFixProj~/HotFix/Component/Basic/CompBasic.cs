// jave.lin 2021/09/22
// 组件的基类

using Assets.Scripts.ILRT;
using HotFix.Behavior;
using UnityEngine;

namespace HotFix.Component.Basic
{
    public class CompBasic : IComp
    {
        public bool IsDisposed { get; private set; }

        public GameObject Owner { get; set; }

        public bool IsStarted { get; private set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
            var goName = Owner == null ? "Null(Disposed)" : Owner.name;
            //ILRTDebug.Log($"{this.GetType().Name}.Dispose(), IsDisposed : {IsDisposed}, GameObject : {goName}");
        }
        public virtual void FixedUpdate()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.FixedUpdate()");
        }
        public virtual void Init()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.Init()");
        }
        public virtual void LateUpdate()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.LateUpdate()");
        }
        public virtual void OnApplicationFocus(bool focus)
        {
            //ILRTDebug.Log($"{this.GetType().Name}.OnApplicationFocus(focus), focus : {focus}");
        }
        public virtual void OnApplicationQuit()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.OnApplicationQuit()");
        }
        public virtual void Update()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.Update()");
            
        }

        public virtual void Start()
        {
            IsStarted = true;
        }
    }
}
