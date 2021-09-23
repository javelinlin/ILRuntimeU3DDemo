// jave.lin 2021/09/22
// 管理器 抽象基类

using Assets.Scripts.ILRT;
using HotFix.Behavior;
using UnityEngine;

namespace HotFix.Mgr.Basic
{
    public abstract class MgrBasic<T> : IBehavior where T : IBehavior, new()
    {
        private static T mInstance = default(T);
        public static T instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new T();
                }
                return mInstance;
            }
        }

        public virtual bool IsDisposed { get; private set; }
        public virtual GameObject Owner { get; set; }

        public virtual bool IsStarted { get; private set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
            //ILRTDebug.Log($"{this.GetType().Name}.Dispose(), IsDisposed : {IsDisposed}");
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

        public virtual void Start()
        {
            IsStarted = true;
        }

        public virtual void Update()
        {
            //ILRTDebug.Log($"{this.GetType().Name}.Update()");
        }
    }
}
