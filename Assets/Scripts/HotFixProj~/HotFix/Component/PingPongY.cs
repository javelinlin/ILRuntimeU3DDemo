// jave.lin 2021/09/22
// Ping Pong Y 的组件

using HotFix.Component.Basic;
using UnityEngine;

namespace HotFix.Component
{
    public class PingPongY : CompBasic
    {
        private Transform trans;

        public override void Dispose()
        {
            trans = null;
            base.Dispose();
        }
        public override void Init()
        {
            base.Init();
            trans = Owner.transform;
        }
        public override void Update()
        {
            base.Update();
            var pos = trans.localPosition;
            pos.y += Mathf.Sin(Time.time) * Time.deltaTime;
            trans.localPosition = pos;
        }
    }
}
