using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Base : MonoBehaviour {

    public static void Quit() { Application.Quit(); }
    public static void ActiveObjectStatic(GameObject go) { UnityThread.executeInUpdate(() => go.SetActive(true)); }
    public static void DeactiveObjectStatic(GameObject go) { UnityThread.executeInUpdate(() => go.SetActive(false)); }

    public static string ColorToHex(Color32 color) { return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString(); }

    public static Color32 HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = 255;
        if(hex.Length > 6)
            a = byte.Parse(hex.Substring(6), System.Globalization.NumberStyles.Number);
        return new Color32(r, g, b, a);
    }
}
