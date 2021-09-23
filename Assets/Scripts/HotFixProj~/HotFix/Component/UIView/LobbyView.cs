// jave.lin 2021/09/22
// 大厅测试 UI类

using HotFix.Component.UIView.Basic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Component.UIView
{
    public class LobbyView : UIViewBasic
    {
        private Button showMessageBoxViewBtn;
        private Button hideMessageBoxViewBtn;
        private Button removeMessageBoxViewBtn;

        public override void Start()
        {
            base.Start();

            showMessageBoxViewBtn = Owner.transform.Find("ShowMessageBoxViewBtn").GetComponent<Button>();
            hideMessageBoxViewBtn = Owner.transform.Find("HideMessageBoxViewBtn").GetComponent<Button>();
            removeMessageBoxViewBtn = Owner.transform.Find("RemoveMessageBoxViewBtn").GetComponent<Button>();

            showMessageBoxViewBtn.onClick.AddListener(OnShowMessageBoxViewBtn);
            hideMessageBoxViewBtn.onClick.AddListener(OnHideMessageBoxViewBtn);
            removeMessageBoxViewBtn.onClick.AddListener(OnRemoveMessageBoxViewBtn);
        }

        private void OnRemoveMessageBoxViewBtn()
        {
            ViewMgr.inst.DisposeView(eViewName.MessageBoxView);
        }

        private void OnHideMessageBoxViewBtn()
        {
            ViewMgr.inst.HideView(eViewName.MessageBoxView);
        }

        private void OnShowMessageBoxViewBtn()
        {
            var view = ViewMgr.inst.ShowView(eViewName.MessageBoxView, Mgr.ViewLayer.Content);
            view.rectTrans.anchoredPosition = Vector2.zero;
        }
    }
}
