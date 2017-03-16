using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OSMLoader
{
    public static string buildUrl(Rect a)
    {
        return "http://api.openstreetmap.org/api/0.6/map?bbox=" + a.x.ToString() + "," + a.y.ToString() + "," + a.width.ToString() + "," + a.height.ToString();
    }

    public static string loadFromRect(Rect a)
    {
        System.Net.WebClient wc = new System.Net.WebClient();
        return wc.DownloadString(buildUrl(a));
    }

    public static string loadFromFile(string path)
    {
        StreamReader sr = File.OpenText("Assets\\Ushia\\OSMData" + "\\" + path);
        string data = sr.ReadToEnd();
        sr.Close();
        return data;
    }
}
