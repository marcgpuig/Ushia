using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OSMLog : ScriptableObject
{
    public static bool logActive = true;
    public static bool logFileActive = true;
    public static string logFile = Application.dataPath + "\\Ushia\\log\\UshiaLog.txt";

    public static void clearLogFile()
    {
        File.WriteAllText(logFile, string.Empty);
    }

    public static void log(string msg)
    {
        if (logActive) Debug.Log(msg);
        if (logFileActive) logToFile("MESSAGE", msg);
    }

    public static void warning(string msg)
    {
        if (logActive) Debug.LogWarning(msg);
        if (logFileActive) logToFile("WARNING", msg);
    }

    public static void error(string msg)
    {
        if (logActive) Debug.LogError(msg);
        if (logFileActive) logToFile("ERROR", msg);
    }

    static void logToFile(string lvl, string msg)
    {
        if (!File.Exists(logFile))
        {
            using (StreamWriter sw = File.CreateText(logFile))
            {
                sw.WriteLine(lvl);
                sw.WriteLine(msg);
                sw.WriteLine("");
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine(lvl);
                sw.WriteLine(msg);
                sw.WriteLine("");
            }
        }
    }
    
}
