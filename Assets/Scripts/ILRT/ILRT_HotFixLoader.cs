#define __ILRT_DEBUG__ // ILRuntime 的断点调试 开关宏

// jave.lin 2021/09/17
// ILRuntime 的 hotfix 部分的加载，基本上写好就不会再去修改，因为只是加载的部分，调试的时候，可以开启 __ILRT_DEBUG__ 宏

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ILRT_HotFixLoader : IDisposable
{
    public Action onLoaded;

    private MemoryStream hotFixDllByteStream;
    private MemoryStream hotFixPdbByteStream;

    public void Dispose()
    {
        if (hotFixDllByteStream != null)
        {
            hotFixDllByteStream.Dispose();
            hotFixDllByteStream = null;
        }
        if (hotFixPdbByteStream != null)
        {
            hotFixPdbByteStream.Dispose();
            hotFixPdbByteStream = null;
        }
        onLoaded = null;
    }

    public IEnumerator Start(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        if (appdomain == null)
        {
            Debug.LogError($"{nameof(ILRT_HotFixLoader)} Start Param's appdomain is null");
            yield break;
        }

        ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider pdbReader = null;

        // 加载 dll
#if UNITY_ANDROID
        string url = Application.streamingAssetsPath + "/HotFix.dll";
#else
        string url = "file:///" + Application.streamingAssetsPath + "/HotFix.dll";
#endif

        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError)
        {
            Debug.Log($"{nameof(ILRT_HotFixLoader)} Download url : {url}, Error : {webRequest.error}");
            yield break;
        }
        byte[] bytes = webRequest.downloadHandler.data;
        hotFixDllByteStream = new MemoryStream(bytes);
        webRequest.Dispose();

#if __ILRT_DEBUG__
        // 加载 pdb
#if UNITY_ANDROID
        url = Application.streamingAssetsPath + "/HotFix.pdb";
#else
        url = "file:///" + Application.streamingAssetsPath + "/HotFix.pdb";
#endif
        webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError)
        {
            Debug.Log($"{nameof(ILRT_HotFixLoader)} Download url : {url}, Error : {webRequest.error}");
            yield break;
        }
        bytes = webRequest.downloadHandler.data;
        hotFixPdbByteStream = new MemoryStream(bytes);
        webRequest.Dispose();

        // 实例 pdb reader
        pdbReader = new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider();
#endif
        try
        {
            appdomain.LoadAssembly(hotFixDllByteStream, hotFixPdbByteStream, pdbReader);
        }
        catch (Exception er)
        {
            Debug.LogError($"ILRT AppDomain 加载程 HotFix 序集失败，Error : {er}");
        }

        onLoaded?.Invoke();
    }
}
