using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class UPlayer : MonoBehaviour
{
    [Header("Chunk parameters:")]
    public float chunkSize = 1;
    public int chunkAdjacentLayers = 1;
    public int maxResidualChunks = 2;

    [Header("World Location:")]
    public double startLon = 0;
    public double startLat = 0;
    public int zoom = 14;
    private USlippyTile startTile;
    private GCS worldStartPoint = new GCS();

    [Header("Clear:")]
    public bool _clearHashMap = false;

    /// Debug Gizmos
    [Header("Debug:")]
    public bool debugChunks = true;
    public bool debugDistances = true;
    public bool debugPositions = true;

    private Hashtable map = new Hashtable();
    private int lastChunkX = 0;
    private int lastChunkY = 0;

    void Start ()
    {
        init();
    }

    public void init()
    {
        worldStartPoint.set(startLon, startLat);
        startTile = new USlippyTile(startLon, startLat, zoom);
        clearHashMap();
    }

    public void clearHashMap()
    {
        foreach (DictionaryEntry p in map)
        {
            DestroyImmediate((GameObject)p.Value);
        }
        map.Clear();
    }

    public bool playerIsInNewChunk()
    {
        int x = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.x);
        int y = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.z);
        if (lastChunkX != x || lastChunkY != y)
        {
            lastChunkX = x;
            lastChunkY = y;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the total quadrants number given the desired 
    /// (Chebyshov distance) adjacent layers.
    /// </summary>
    public int nQuadrants()
    {
        return (int)System.Math.Pow(((2 * chunkAdjacentLayers) + 1), 2);
    }

    /// <summary>
    /// Class for store the (Chebyshov) distance from the player 
    /// to each chunk and then remove the farest chunk/s.
    /// </summary>
    private class distChunk
    {
        public int dist;
        public string chunk;
        public distChunk(int _dist = 0, string _chunk = "")
        {
            dist = _dist;
            chunk = _chunk;
        }
    }

    private void removeResidualChunks()
    {
        int totalAvailableChunks = nQuadrants() + maxResidualChunks;
        List<distChunk> distances = new List<distChunk>();

        foreach (DictionaryEntry p in map)
        {
            GameObject gameObj = (GameObject)p.Value;

            GameObject t = (GameObject)p.Value;
            Int3 chunk = Int3.findChunk(t.transform.position + new Vector3(chunkSize*0.5f, chunkSize * 0.5f, chunkSize * 0.5f), chunkSize);
            Int3 player = Int3.findChunk(transform.position, chunkSize);

            /// Distancia de Chebyshov
            /// https://es.wikipedia.org/wiki/Distancia_de_Chebyshov
            Int3 dist = chunk - player;
            dist = dist.abs();

            distances.Add(new distChunk(Mathf.Max(dist.x, dist.z), (string)p.Key));
        }

        List<distChunk> sortedDistances = distances.OrderByDescending(o => o.dist).ToList();

        int i = map.Count;
        foreach (distChunk d in sortedDistances)
        {
            if (i <= totalAvailableChunks) return;
            if (map.ContainsKey(d.chunk))
            {
                DestroyImmediate((GameObject)map[d.chunk]);
                map.Remove(d.chunk);
            }
            i--;
        }
    }

    public static string genTerrainName(int x, int y)
    {
        return "Terrain_(" + x + "," + y + ")";
    }

    public GameObject createTerrain(USlippyTile tile, string key)
    {
        GameObject t = new GameObject(key);

        TerrainData tData = new TerrainData();
        tData.size = new Vector3(chunkSize / 8, 0, chunkSize / 8);

        TerrainCollider tColliderComp = t.AddComponent<TerrainCollider>();
        Terrain tComp = t.AddComponent<Terrain>();
        tColliderComp.terrainData = tData;
        tComp.terrainData = tData;

        t.AddComponent<UTerrain>();
        t.GetComponent<UTerrain>().genHeight(tile);

        return t;
    }

    void Update()
    {
        if(_clearHashMap)
        {
            clearHashMap();
            _clearHashMap = false;
        }

        if(playerIsInNewChunk())
        {
            /// Chunk Position
            int x = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.x);
            int y = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.z);

            /// Chunk Number (id)
            int xNum = (int)(x / chunkSize);
            int yNum = (int)(y / chunkSize);

            /// Starting tile
            startTile = new USlippyTile(startLon, startLat, zoom);

            /// Sapwn all the new needed chunks
            for (int i = -chunkAdjacentLayers; i <= chunkAdjacentLayers; i++)
            {
                for (int j = -chunkAdjacentLayers; j <= chunkAdjacentLayers; j++)
                {
                    USlippyTile sTile = new USlippyTile(xNum + i + startTile.x, yNum + j + startTile.y, zoom);
                    string key = genTerrainName(sTile.x, sTile.y);
                    if (!map.ContainsKey(key))
                    {
                        GameObject t = createTerrain(sTile, key);

                        Vector3 pos = new Vector3();
                        pos.Set(x + (i * chunkSize), 0, y + (j * chunkSize));
                        t.GetComponent<Transform>().position = pos;

                        map.Add(key, t);
                    }
                }
            }
            /// After spawning the new chunks, remove the olders that we dont need
            removeResidualChunks();
        }
    }

    void drawRect(Vector3 pos, float size)
    {
        Gizmos.DrawLine(pos, pos + new Vector3(size, 0, 0));
        Gizmos.DrawLine(pos, pos + new Vector3(0, 0, size));
        Gizmos.DrawLine(pos + new Vector3(size, 0, 0), pos + new Vector3(size, 0, size));
        Gizmos.DrawLine(pos + new Vector3(0, 0, size), pos + new Vector3(size, 0, size));
    }

    public void renderRects()
    {
        float xx = GetComponent<Transform>().position.x % chunkSize;
        float yy = GetComponent<Transform>().position.z % chunkSize;

        for (int i = -chunkAdjacentLayers; i <= chunkAdjacentLayers; i++)
        {
            for (int j = -chunkAdjacentLayers; j <= chunkAdjacentLayers; j++)
            {
                Vector3 chunkPos = new Vector3(xx + (chunkSize * i) + (GetComponent<Transform>().position.x < 0 ? chunkSize : 0), 0, yy + (chunkSize * j) + (GetComponent<Transform>().position.z < 0 ? chunkSize : 0));
                chunkPos = GetComponent<Transform>().position - chunkPos;
                drawRect(chunkPos, chunkSize);
            }
        }
    }

    public void drawPoints(float size = 1)
    {
        int x = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.x);
        int y = UMaths.scaledFloor(chunkSize, GetComponent<Transform>().position.z);

        for (int i = -chunkAdjacentLayers; i <= chunkAdjacentLayers; i++)
        {
            for (int j = -chunkAdjacentLayers; j <= chunkAdjacentLayers; j++)
            {
                Vector3 chunkPos = new Vector3(x + (i * chunkSize), 0, y + (j * chunkSize));
                Gizmos.DrawSphere(chunkPos, size);
            }
        }
    }

    public void drawDistances()
    {
        float halfHypot = (float)(chunkSize/2.0f);
        foreach (DictionaryEntry p in map)
        {
            GameObject gameObj = (GameObject)p.Value;
            Gizmos.DrawLine(GetComponent<Transform>().position, gameObj.transform.position + new Vector3(halfHypot, 0, halfHypot));
        }
    }

    private void OnDrawGizmos()
    {
        if (debugChunks)
        {
            Gizmos.color = Color.green;
            renderRects();
        }
        if (debugDistances)
        {
            Gizmos.color = Color.cyan;
            drawDistances();
        }
        if (debugPositions)
        {
            Gizmos.color = Color.blue;
            drawPoints(chunkSize * 0.025f);
        }
        Gizmos.DrawIcon(GetComponent<Transform>().position, "UshiaLocation.png");
    }
}