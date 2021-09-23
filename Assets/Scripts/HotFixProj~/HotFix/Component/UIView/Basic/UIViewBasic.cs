// jave.lin 2021/09/22
// UI 基类

using HotFix.Behavior;
using UnityEngine;

namespace HotFix.Component.UIView.Basic
{
    public abstract class UIViewBasic : IComp, IUIView
    {
        public GameObject Owner { get; set; }

        public bool IsDisposed { get; private set; }

        public bool IsStarted { get; private set; }

        public RectTransform rectTrans
        {
            get;
            private set;
        }

        public virtual void BringTop()
        {
            
        }

        public virtual void Dispose()
        {
            IsDisposed = true;

            rectTrans = null;
            Owner = null;
        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void Hide()
        {
            if (Owner.activeSelf)
            {
                Owner.SetActive(false);
            }
        }

        public virtual void Init()
        {
            rectTrans = Owner.GetComponent<RectTransform>();

        }

        public virtual void LateUpdate()
        {
            
        }

        public virtual void OnApplicationFocus(bool focus)
        {
            
        }

        public virtual void OnApplicationQuit()
        {
            
        }

        public virtual void Show()
        {
            if (!Owner.activeSelf)
            {
                Owner.SetActive(true);
            }
        }

        public virtual void Show(Transform parent)
        {
            Owner.transform.SetParent(parent);
            if (!Owner.activeSelf)
            {
                Owner.SetActive(true);
            }
        }

        public virtual void Start()
        {
            IsStarted = true;

        }

        public virtual void Update()
        {
            
        }
    }
}
