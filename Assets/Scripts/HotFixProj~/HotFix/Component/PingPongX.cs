// jave.lin 2021/09/22
// Ping Pong X 的组件

using HotFix.Component.Basic;
using UnityEngine;

namespace HotFix.Component
{
    public class PingPongX : CompBasic
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
            pos.x += Mathf.Sin(Time.time) * Time.deltaTime;
            trans.localPosition = pos;
        }
    }
}
