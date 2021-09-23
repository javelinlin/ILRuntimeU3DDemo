// jave.lin 2021/09/17
// 模块 mgr 的接口

namespace HotFix.Behavior
{
    public interface IBehavior
    {
        bool IsStarted { get; }
        bool IsDisposed { get; }
        void Dispose();
        void Init();
        void Start();
        void Update();
        void LateUpdate();
        void FixedUpdate();
        void OnApplicationFocus(bool focus);
        void OnApplicationQuit();
    }
}
