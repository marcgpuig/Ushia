using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSMDebug : MonoBehaviour
{
    public string filePath;
    public OSMChunk chunk;

    [Range(0.0f, 10.5f)]
    public float nodeSize = 0.1f;
    private Vector3 sNodes;
    public Color cNodes = new Color(1,1,1,1);

    void Start ()
    {
        chunk = new OSMChunk();
        chunk.loadOSM(filePath);
        chunk.loadNodes();

        sNodes = new Vector3(nodeSize, nodeSize, nodeSize);
        /*foreach (OSMNode n in chunk.nodes)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            c.transform.position = n.pos;
        }*/
    }

    private void OnDrawGizmos()
    {
        /// Draw chunk size
        //Gizmos.DrawLine()

        sNodes.Set(nodeSize, nodeSize, nodeSize);
        Gizmos.color = cNodes;
        //Gizmos.DrawCube(new Vector3(0, 0, 0), sNodes);

        if (chunk != null && chunk.nodes != null)
            foreach (OSMNode n in chunk.nodes)
            {
                Gizmos.DrawCube(n.pos, sNodes);
            }
    }
}
