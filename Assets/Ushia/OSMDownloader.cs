using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;

public class OSMDownloader : MonoBehaviour
{
    public Rect area;
    float minLon = float.MaxValue, maxLon = float.MinValue;
    float minLat = float.MaxValue, maxLat = float.MinValue;
    private List<Vector2> nodes = new List<Vector2>();

    public string file = "";

    void Start () {

        
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (Vector2 v in nodes)
        {
            Gizmos.DrawSphere(new Vector3(v.x, 0.0f, v.y), 0.001f);
        }
        float aff = 10;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(area.xMin * aff, 0, area.yMin * aff), new Vector3(area.xMax * aff, 0, area.yMin * aff));
        Gizmos.DrawLine(new Vector3(area.xMin * aff, 0, area.yMin * aff), new Vector3(area.xMin * aff, 0, area.yMax * aff));


        //Gizmos.DrawLine(area.y, area.height);
    }
}
