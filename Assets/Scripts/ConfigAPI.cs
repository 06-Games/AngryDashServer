using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigAPI
{
#if UNITY_EDITOR
    static string configPath = "C:\\Users\\evan\\Documents\\Unity\\Compiller\\Angry Dash Server\\"+ Application.version + "\\config.ini";
#elif UNITY_STANDALONE
        static string[] Path = Application.dataPath.Split(new string[2] { "/", "\\" }, System.StringSplitOptions.None);
        static string configPath = Application.dataPath.Replace(Path[Path.Length - 1], "") + "/config.ini";
#else
    static string configPath = Application.dataPath + "/config.ini";
#endif

    static string[] config = null;
    public static void Reload()
    {
        if (!File.Exists(configPath))
            File.WriteAllLines(configPath, new string[4] { "# Angry Dash Server config file", "# 06Games,", "# All rights reserved", "" });
        config = File.ReadAllLines(configPath);

        Log.Reload();
    }

    public static string GetString(string d)
    {
        string id = d + " = ";
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].Contains(id))
                    return config[i].Replace(id, "");
            }
        return null;
    }

    public static bool GetBool(string d)
    {
        bool b = false;
        try { b = bool.Parse(GetString(d)); } catch { }
        return b;
    }

    public static int GetInt(string d)
    {
        int b = 0;
        try { b = int.Parse(GetString(d)); } catch { }
        return b;
    }

    public static float GetFloat(string d)
    {
        float b = 0;
        try { b = float.Parse(GetString(d)); } catch { }
        return b;
    }

    public static bool ParamExist(string d)
    {
        string id = d + " = ";
        if (config != null)
        {
            int l = -1;
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].Contains(id))
                    l = i;
            }

            return l > -1;
        }
        else return false;
    }
    public static void SetString(string d, string p)
    {
        string id = d + " = ";

        string[] lines = new string[4] { "# Angry Dash Server config file", "# 06Games,", "# All rights reserved", "" };
        if (config != null)
            lines = config;
        int l = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(id))
                l = i;
        }

        if (l == -1)
            lines = lines.Union(new string[1] { id + p }).ToArray();
        else lines[l] = id + p;

        File.WriteAllLines(configPath, lines);
        config = lines;
    }

    public static void SetBool(string d, bool p) { SetString(d, p.ToString()); }
    public static void SetInt(string d, int p) { SetString(d, p.ToString()); }
    public static void SetFloat(string d, float p) { SetString(d, p.ToString()); }
}
