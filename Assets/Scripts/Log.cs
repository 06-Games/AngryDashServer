using Microsoft.Win32;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Net;
using System.ComponentModel;
using UnityEngine.UI;
using System.Linq;

public class Log : MonoBehaviour {
    
    
    void OnEnable() { Application.logMessageReceived += HandleLog; }
    void OnDisable() { Application.logMessageReceived -= HandleLog; }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
#if UNITY_EDITOR
        UnityThread.executeInUpdate(() =>
        {
            string[] trace = stackTrace.Split(new string[1] { "\n" }, StringSplitOptions.None);
            stackTrace = "";
            for (int i = 0; i < trace.Length - 1; i++)
                stackTrace = stackTrace + "\n\t\t" + trace[i];

            string fileText = "";
            if (File.Exists(pathToActualLogMessage()))
                fileText = File.ReadAllText(pathToActualLogMessage());

            string current = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + logString + stackTrace + "\n\n";
            string line = fileText + current;
            File.WriteAllText(pathToActualLogMessage(), line);
        });
#endif
    }
    public static void LogNewMessage(string logString, bool inEditor = false, string stackTrace = null)
    {
        bool go = true;
#if UNITY_EDITOR
        go = inEditor;
#endif
        if (go)
        {
            UnityThread.executeInUpdate(() =>
            {
                if (stackTrace != null)
                {
                    string[] trace = stackTrace.Split(new string[1] { "\n" }, StringSplitOptions.None);
                    stackTrace = "";
                    for (int i = 0; i < trace.Length - 1; i++)
                        stackTrace = stackTrace + "\n\t\t" + trace[i];
                }

                string fileText = "";
                if (File.Exists(pathToActualLogMessage()))
                    fileText = File.ReadAllText(pathToActualLogMessage());

                string current = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + logString + stackTrace + "\n";
                string line = fileText + current;
                File.WriteAllText(pathToActualLogMessage(), line);
            });
        }
    }
    public static string pathToActualLogMessage() {
        string path = Application.dataPath;
#if UNITY_EDITOR
        path = "C:\\Users\\evan\\Documents\\Unity\\Compiller\\Angry Dash Server\\1.0\\logs\\";
#elif UNITY_STANDALONE
        string[] Path = Application.dataPath.Split(new string[2] { "/", "\\" }, System.StringSplitOptions.None);
        path = Application.dataPath.Replace(Path[Path.Length - 1], "") + "/logs/";
#endif
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string DT = (DateTime.Now - TimeSpan.FromSeconds(Time.realtimeSinceStartup)).ToString("yyyy-MM-dd HH-mm-ss");
        return path + DT + ".log";
    }

    void Awake()
    {
        UnityThread.initUnityThread();
    }
}
