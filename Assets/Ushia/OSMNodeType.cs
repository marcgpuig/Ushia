using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSMNodeTypes
{
    public struct OSMNodeType
    {
        public string name;
        public Color debugColor;
    }

    public static List<OSMNodeType> list;

    public static bool nameExists(string n)
    {
        foreach (OSMNodeType nt in list)
            if (nt.name == n) return true;
        return false;
    }

    public static bool add(string _name, Color _color)
    {
        /// check if the name does not exists
        if (nameExists(_name)) return false;

        OSMNodeType t;
        t.name = _name;
        t.debugColor = _color;
        list.Add(t);
        return true;
    }

    public static void clearList()
    {
        list.Clear();
    }
}
