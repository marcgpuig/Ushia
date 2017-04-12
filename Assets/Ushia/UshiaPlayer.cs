using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UshiaPlayer : MonoBehaviour
{
    private Transform tr = null;
    public float chunkSize = 1;
    public int chunkAdjacentLayers = 1;
    public double startLon = 0, startLat = 0;
    public GCS worldStartPoint = new GCS();

    void Start ()
    {
        tr = GetComponent<Transform>();
        worldStartPoint.set(startLon, startLat);
    }
	
	void Update ()
    {
        tr = GetComponent<Transform>();
    }

    void drawRect(Vector3 pos, float size)
    {
        Gizmos.DrawLine(pos, pos + new Vector3(size, 0, 0));
        Gizmos.DrawLine(pos, pos + new Vector3(0, 0, size));
        Gizmos.DrawLine(pos + new Vector3(size, 0, 0), pos + new Vector3(size, 0, size));
        Gizmos.DrawLine(pos + new Vector3(0, 0, size), pos + new Vector3(size, 0, size));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float xx = tr.position.x % chunkSize;
        float yy = tr.position.z % chunkSize;

        for (int i = -chunkAdjacentLayers; i <= chunkAdjacentLayers; i++)
        {
            for (int j = -chunkAdjacentLayers; j <= chunkAdjacentLayers; j++)
            {
                Vector3 chunkPos = new Vector3(xx + (chunkSize * i) + (tr.position.x < 0 ? chunkSize : 0), 0, yy + (chunkSize * j) + (tr.position.z < 0 ? chunkSize : 0));
                chunkPos = tr.position - chunkPos;
                drawRect(chunkPos, chunkSize);
            }
        }
        Gizmos.DrawIcon(tr.position, "UshiaLocation.png");
    }
}