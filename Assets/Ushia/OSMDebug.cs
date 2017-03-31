using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
        chunk.loadWays();

        sNodes = new Vector3(nodeSize, nodeSize, nodeSize);
        /*foreach (OSMNode n in chunk.nodes)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            c.transform.position = n.pos;
        }*/
    }

    private void OnDrawGizmosSelected()
    {
        /// Draw chunk rect
        if (chunk != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(chunk.virtualWorldPos, chunk.virtualWorldPos + new Vector3((float)chunk.width, 0, 0));
            Gizmos.DrawLine(chunk.virtualWorldPos, chunk.virtualWorldPos + new Vector3(0, 0, (float)chunk.height));
            Gizmos.DrawLine(chunk.virtualWorldPos + new Vector3((float)chunk.width, 0, 0), chunk.virtualWorldPos + new Vector3((float)chunk.width, 0, (float)chunk.height));
            Gizmos.DrawLine(chunk.virtualWorldPos + new Vector3(0, 0, (float)chunk.height), chunk.virtualWorldPos + new Vector3((float)chunk.width, 0, (float)chunk.height));
        }
    }

    private void OnDrawGizmos()
    {
        sNodes.Set(nodeSize, nodeSize, nodeSize);
        Gizmos.color = cNodes;
        //Gizmos.DrawCube(new Vector3(0, 0, 0), sNodes);

        if (chunk != null && chunk.nodes != null)
        {
            /// drawing all nodes
            foreach (DictionaryEntry e in chunk.nodes)
            {
                OSMNode n = (OSMNode)e.Value;
                Gizmos.DrawCube(n.pos, sNodes);
            }

            /// drawing all ways
            OSMNode current;
            OSMNode next;
            foreach (DictionaryEntry e in chunk.ways)
            {
                OSMWay w = (OSMWay)e.Value;

                Color old = Gizmos.color;
                if (w.tags.ContainsKey("waterway"))
                {
                    if (w.tags.ContainsValue("river"))
                        Gizmos.color = new Color(0.0f, 0.0f, 1f);
                    if (w.tags.ContainsValue("stream"))
                        Gizmos.color = new Color(0.2f, 0.2f, 0.8f);
                }

                if (w.tags.ContainsKey("highway"))
                {
                    if (w.tags.ContainsValue("primary"))
                        Gizmos.color = new Color(1f, 1f, 1f);
                    if (w.tags.ContainsValue("secondary"))
                        Gizmos.color = new Color(0.85f, 0.85f, 0.85f);
                    if (w.tags.ContainsValue("tertiary"))
                        Gizmos.color = new Color(0.75f, 0.75f, 0.75f);
                }

                if (w.tags.ContainsKey("power"))
                {
                    if (w.tags.ContainsValue("line"))
                        Gizmos.color = new Color(0.95f, 0.85f, 0.2f);
                }

                for (int i = 0; i < w.nodesIds.Count - 1; i++)
                {
                    current = (OSMNode)chunk.nodes[w.nodesIds[i]];
                    next    = (OSMNode)chunk.nodes[w.nodesIds[i+1]];
                    Gizmos.DrawLine(current.pos, next.pos);
                }

                Gizmos.color = old;
            }
        }
    }
}
