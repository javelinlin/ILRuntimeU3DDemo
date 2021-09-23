// jave.lin 2021/09/22
// UI 的接口

using UnityEngine;

namespace HotFix.Component.UIView.Basic
{
    public interface IUIView
    {
        RectTransform rectTrans { get; }
        void Show();
        void Show(Transform parent);
        void Hide();
        void BringTop();
    }
}
