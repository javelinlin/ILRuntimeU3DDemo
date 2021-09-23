// jave.lin 2021/09/22
// ILRuntime 的调试器 窗口

using System;
using UnityEditor;
using UnityEngine;

public class ILRTDebuggerWindow : EditorWindow
{
    [MenuItem("实用工具/ILRuntime/ILRuntime调试窗口")]
    public static void _Show()
    {
        var win = EditorWindow.GetWindow<ILRTDebuggerWindow>();
        win.titleContent = new GUIContent("ILRuntime调试窗口");
        win.Show();
    }

    public static bool BeforeRunIsAttached
    {
        get; private set;
    }

    public static ILRuntime.Runtime.Enviorment.AppDomain CreatedDomain
    {
        get; private set;
    }

    private int port = 56000;
    private bool? startSuccess = null;

    private void OnGUI()
    {
        if (ILRT_DEBUG_BRIDGE.domain == null)
        {
            if (Application.isPlaying)
            {
                if (ILRTBehaviorLauncher.inst != null && ILRTBehaviorLauncher.inst.domain != null)
                {
                    ILRT_DEBUG_BRIDGE.Set(ILRTBehaviorLauncher.inst.domain);
                }
                else
                {
                    //ILRT_DEBUG_BRIDGE.Create();
                    //CreatedDomain = ILRT_DEBUG_BRIDGE.domain;
                }
            }
            else
            {
                //ILRT_DEBUG_BRIDGE.Create();
                //CreatedDomain = ILRT_DEBUG_BRIDGE.domain;
            }
        }
        var srcCol = GUI.contentColor;

        var domain = ILRT_DEBUG_BRIDGE.domain;
        if (domain == null)
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("Waiting for ILRuntime AppDomain Create.");
            GUI.contentColor = srcCol;
            return;
        }

        var srcEnabled = GUI.enabled;
        var srcBgCol = GUI.backgroundColor;

        GUI.enabled = !domain.DebugService.IsStarted;
        port = domain.DebugService.Port;
        port = port == -1 ? 56000 : port;
        port = EditorGUILayout.IntField("Listening Port", port);

        GUI.enabled = !domain.DebugService.IsStarted;
        GUI.backgroundColor = GUI.enabled ? Color.green: Color.green * 0.5f;
        if (GUILayout.Button("Start Debug Server"))
        {
            try
            {
                startSuccess = domain.DebugService.StartDebugService(port);
            }
            catch (Exception er)
            {
                Debug.LogError($"{er}");
                startSuccess = false;
            }
        }
        GUI.enabled = domain.DebugService.IsStarted;
        GUI.backgroundColor = GUI.enabled ? Color.red : Color.red * 0.5f;
        if (GUILayout.Button("Stop Debug Server"))
        {
            domain.DebugService.StopDebugService();
        }

        // status
        GUI.enabled = srcEnabled;
        if (domain.DebugService.IsDebuggerAttached)
        {
            GUI.contentColor = Color.green;
            GUILayout.Label("Debug Server Checked : Attached");
        }
        else
        {
            if (startSuccess.HasValue && !startSuccess.Value)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("Debug Server Start Failure.");
            }
            GUI.contentColor = Color.gray;
            GUILayout.Label("Debug Server Checked : Not Attached");
        }

        BeforeRunIsAttached = domain.DebugService.IsDebuggerAttached;
        GUI.backgroundColor = srcBgCol;
        GUI.contentColor = srcCol;
    }
}
