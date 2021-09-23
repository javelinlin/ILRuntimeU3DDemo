// jave.lin 2021/09/22
// 小心对话框的测试类

using Assets.Scripts.ILRT;
using HotFix.Component.UIView.Basic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Component.UIView
{
    public class MessageBoxView : UIViewBasic
    {
        private Image bg;
        private Text text;
        private Button noBtn;
        private Button yesBtn;

        public override void Init()
        {
            base.Init();

            bg = Owner.transform.Find("Bg").GetComponent<Image>();
            text = Owner.transform.Find("Text").GetComponent<Text>();
            noBtn = Owner.transform.Find("NoBtn").GetComponent<Button>();
            yesBtn = Owner.transform.Find("YesBtn").GetComponent<Button>();

            bg.color = new Color(0.0f, 0.0f, 0.5f, 0.5f);
            text.text = $"Hello This is [{GetType().Name}] HotFix Project Script Holder.";
            noBtn.onClick.AddListener(onNoBtnClick);
            yesBtn.onClick.AddListener(onYesBtnClick);
        }

        //public override void Start()
        //{
        //    base.Start();


        //}

        public override void Dispose()
        {
            noBtn.onClick.RemoveListener(onNoBtnClick);
            yesBtn.onClick.RemoveListener(onYesBtnClick);

            bg = null;
            text = null;
            noBtn = null;
            yesBtn = null;

            base.Dispose();
        }

        private void onYesBtnClick()
        {
            ILRTDebug.Log($"[{GetType().Name} onYesBtnClick");
            Hide();
        }

        private void onNoBtnClick()
        {
            ILRTDebug.Log($"[{GetType().Name} onNoBtnClick");
            Hide();
        }
    }
}
