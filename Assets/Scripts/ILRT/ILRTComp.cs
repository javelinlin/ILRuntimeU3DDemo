// jave.lin 2021/09/22
// ILRT 组件类
// ILRTComp 继承自 MonoBehavior
// 而且 真正 Replace 处理入口在 Awake
// 也就是说需要依赖 MonoBehavior 生命周期特性的 Awake 来处理
// 如果说某个 GameObject 下添加了 ILRTComp 组件
// 但是这个本身的 GameObject.selfActive == false
// 那么MonoBehavior 的 Awake 是不会触发的，直到 GameObject.activeInHierarchy == true 才会触发
// 因此，为了解决这个问题，我们在 HotFixLauncher 中添加了 InternalAllReplaceComp(GameObject go) 方法来主动处理

using System;
using UnityEngine;

public class ILRTComp : MonoBehaviour
{
    public string ILRT_comp_name;
    public bool HadReplaced
    {
        get;
        private set;
    }
    void Awake()
    {
        Debug.Log($"{nameof(ILRTComp)}.Awake ILRT_comp_name : {ILRT_comp_name}, GameObject.name : {gameObject.name}");
        if (string.IsNullOrEmpty(ILRT_comp_name))
        {
            Debug.LogError($"{nameof(ILRTComp)}.ILRT_comp_name : [{ILRT_comp_name}] is null or Empty, GameObject.name : {gameObject.name}");
            return;
        }
        if (ILRTBehaviorLauncher.inst.hotFixInited)
        {
            try
            {
                ILRTCompReplaceHelper.Replace(this, ILRTBehaviorLauncher.inst.domain);
                HadReplaced = true;
            }
            catch (Exception er)
            {
                HadReplaced = false;
                Debug.LogError($"{nameof(ILRTComp)}.ILRT_comp_name : [{ILRT_comp_name}], GameObject.name : {gameObject.name}, Error : {er}");
            }
        }
        else
        {
            ILRTCompReplaceHelper.PushToPendingList(this);
        }
    }
}
