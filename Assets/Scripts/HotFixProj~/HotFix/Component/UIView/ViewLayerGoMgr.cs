// jave.lin 2021/09/23
// UI 层级的 GO 管理器

using UnityEngine;

namespace HotFix.Mgr
{
    public enum ViewLayer
    {
        Content,        // 内容层
        Dialog,         // 对话层
        Tips,           // 提示层
        CustomSpecial,  // 用户指定层
    }
    public static class ViewLayerGoMgr
    {
        public static GameObject content;
        public static GameObject dialog;
        public static GameObject tips;
        // jave.lin : 如果还有需要其他的层级可以自行在此加上即可
        public static Transform GetUseParent(ViewLayer layer, Transform parent)
        {
            Transform ret = null;
            if (layer == ViewLayer.CustomSpecial)
            {
                ret = parent;
            }
            else
            {
                switch (layer)
                {
                    case ViewLayer.Content:
                        ret = content.transform;
                        break;
                    case ViewLayer.Dialog:
                        ret = dialog.transform;
                        break;
                    case ViewLayer.Tips:
                        ret = tips.transform;
                        break;
                }
            }
            return ret;
        }
    }
}
