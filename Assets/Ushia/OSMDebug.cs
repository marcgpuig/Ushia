using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(OSMChunk))]
public class OSMDebug : MonoBehaviour
{
    private OSMChunk chunk;

    //[Range(0.0f, 10.5f)]
    public float nodeSize = 0.1f;
    private Vector3 sNodes;
    public Color cNodes = new Color(1,0,1,0.3f);

    public Vector3 offset;

    public bool debugNodes = false;
    public bool debugWays = true;

    void Start()
    {
        sNodes = new Vector3(nodeSize, nodeSize, nodeSize);
        chunk = GetComponent<OSMChunk>();
        offset = new Vector3(0, 44000, 0);
    }

    private void Update()
    {

    }

    private void drawChunkBounds()
    {
        Gizmos.DrawLine(chunk.transform.position, chunk.transform.position + new Vector3((float)chunk.width, 0, 0));
        Gizmos.DrawLine(chunk.transform.position, chunk.transform.position + new Vector3(0, 0, (float)chunk.height));
        Gizmos.DrawLine(chunk.transform.position + new Vector3((float)chunk.width, 0, 0), chunk.transform.position + new Vector3((float)chunk.width, 0, (float)chunk.height));
        Gizmos.DrawLine(chunk.transform.position + new Vector3(0, 0, (float)chunk.height), chunk.transform.position + new Vector3((float)chunk.width, 0, (float)chunk.height));
    }

    private void OnDrawGizmosSelected()
    {
        /// Draw chunk rect
        if (chunk != null)
        {
            Gizmos.color = Color.green;
            drawChunkBounds();
        }
    }

    private void OnDrawGizmos()
    {
        sNodes.Set(nodeSize, nodeSize, nodeSize);
        Gizmos.color = cNodes;

        Vector3 pos = transform.position + offset;

        if (chunk.haveParser)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(250, 0, 250));
            //drawChunkBounds();
        }

        //Gizmos.DrawCube(new Vector3(0, 0, 0), sNodes
        if (chunk != null && chunk.isLoaded && chunk.nodes != null)
        {
            if (debugNodes)
            {
                /// drawing all nodes
                foreach (DictionaryEntry e in chunk.nodes)
                {
                    OSMNode n = (OSMNode)e.Value;
                    Gizmos.DrawCube(n.pos + pos, sNodes);
                }
            }

            if (debugWays)
            {
                /// drawing all ways
                OSMNode current;
                OSMNode next;
                foreach (DictionaryEntry e in chunk.ways)
                {
                    OSMWay w = (OSMWay)e.Value;

                    Color old = Gizmos.color;

                    if (w.tags.ContainsKey("highway"))
                    {
                        if (w.tags.ContainsValue("motorway") || w.tags.ContainsValue("motorway_link"))
                            Gizmos.color = new Color(0.95f, 0.93f, 0.91f);
                        if (w.tags.ContainsValue("trunk") || w.tags.ContainsValue("trunk_link"))
                            Gizmos.color = new Color(0.98f, 0.69f, 0.60f);
                        if (w.tags.ContainsValue("primary") || w.tags.ContainsValue("primary_link"))
                            Gizmos.color = new Color(0.95f, 0.93f, 0.91f);
                        if (w.tags.ContainsValue("secondary") || w.tags.ContainsValue("secondary_link"))
                            Gizmos.color = new Color(0.95f, 0.93f, 0.91f);
                        if (w.tags.ContainsValue("tertiary") || w.tags.ContainsValue("tertiary_link"))
                            Gizmos.color = new Color(0.75f, 0.75f, 0.75f);
                        if (w.tags.ContainsValue("unclassified"))
                            Gizmos.color = new Color(0.60f, 0.75f, 0.75f);
                        if (w.tags.ContainsValue("residential"))
                            Gizmos.color = new Color(0.52f, 0.52f, 0.52f);
                        if (w.tags.ContainsValue("service"))
                            Gizmos.color = new Color(0.75f, 0.75f, 0.6f);

                        /// mainly/exclusively for pedestrians
                        if (w.tags.ContainsValue("footway") || w.tags.ContainsValue("bridleway") || w.tags.ContainsValue("steps") || w.tags.ContainsValue("path") || w.tags.ContainsValue("pedestrian"))
                            Gizmos.color = new Color(0.827f, 0.615f, 0.070f, 0.5f);
                    }

                    if (w.tags.ContainsKey("waterway"))
                    {
                        if (w.tags.ContainsValue("river"))
                            Gizmos.color = new Color(0.1f, 0.2f, 0.9f);
                        if (w.tags.ContainsValue("stream"))
                            Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
                    }

                    if (w.tags.ContainsKey("craft") || w.tags.ContainsKey("building"))
                    {
                        Gizmos.color = new Color(0.8f, 0.4f, 0.1f, 0.8f);
                    }

                    if (w.tags.ContainsKey("railway"))
                    {
                        Gizmos.color = new Color(0.713f, 0.827f, 0.070f, 0.6f);
                    }

                    if (w.tags.ContainsKey("power"))
                    {
                        if (w.tags.ContainsValue("line"))
                            Gizmos.color = new Color(0.95f, 0.85f, 0.2f);
                    }

                    for (int i = 0; i < w.nodesIds.Count - 1; i++)
                    {
                        current = (OSMNode)chunk.nodes[w.nodesIds[i]];
                        next = (OSMNode)chunk.nodes[w.nodesIds[i + 1]];
                        Gizmos.DrawLine(current.pos + pos, next.pos + pos);
                    }

                    Gizmos.color = old;
                }
            }
        }
        else if (chunk == null)
        {
            Debug.Log("Debug chunk is null");
        }
    }   
}
