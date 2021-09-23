// jave.lin 2021/09/22
// ILRT 的组件接口

using UnityEngine;

namespace HotFix.Behavior
{
    public interface IComp : IBehavior
    {
        GameObject Owner { get; set; }
    }
}
