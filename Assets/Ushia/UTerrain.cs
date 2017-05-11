using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
public class UTerrain : MonoBehaviour {

    public static float terrainDepth = -4400*10;
    public static float terrainHeight = 8848;

    public bool generated = false;

    private UTerrainLoader loader;
    public USlippyTile tile;
    private UPlayer player;

    private bool borderXFixed = false;
    private bool borderYFixed = false;
    private bool edgeFixed = false;
    private bool fullFixed = false;

    private Terrain[] neighbors; /// left, top, right, bottom
    
    public bool isFullyLoaded { get { return generated && fullFixed; } }

    public void init(USlippyTile t, UPlayer p)
    {
        tile = t;
        player = p;
    }

    /// <summary>
    /// Deletes this Terrain
    /// </summary>
    public void destroy()
    {
        DestroyImmediate(transform.root.gameObject);
    }

    /// <summary>
    /// Set terrain neighbors that fixes the LOD
    /// </summary>
    private void updateNeightbors()
    {
        /// left, top, right, bottom
        GetComponent<Terrain>().SetNeighbors(neighbors[0], neighbors[1], neighbors[2], neighbors[3]);
        GetComponent<Terrain>().Flush();
    }

    public void setLeftTerrain(Terrain t)
    {
        neighbors[0] = t;
        updateNeightbors();
    }

    public void setTopTerrain(Terrain t)
    {
        neighbors[1] = t;
        updateNeightbors();
    }

    public void setRightTerrain(Terrain t)
    {
        neighbors[2] = t;
        updateNeightbors();
    }

    public void setBottomTerrain(Terrain t)
    {
        neighbors[3] = t;
        updateNeightbors();
    }

    /// <summary>
    /// Fixes the Northern borders
    /// </summary>
    /// <param name="t">Northern (Top) terrain</param>
    public void fixBorderX(Terrain t)
    {
        /// get this Terrain's height data
        TerrainData data  = GetComponent<Terrain>().terrainData;
        float[,] heights  = data.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        /// get the other Terrain's height data
        TerrainData data2 = t.terrainData;
        float[,] heights2 = data2.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        /// set the terrain neighbors
        setTopTerrain(t);
        t.GetComponent<UTerrain>().setBottomTerrain(GetComponent<Terrain>());

        for (int i = 0; i < data.alphamapWidth; i++)
            heights[data.alphamapWidth - 1, i] = heights2[0, i];

        /// Apply the border height changes to the terrain data and then update the terrain
        data.SetHeights(0, 0, heights);
        GetComponent<Terrain>().ApplyDelayedHeightmapModification();
    }

    /// <summary>
    /// Fixes the Eastern borders
    /// </summary>
    /// <param name="t">Eastern (Right) terrain</param>
    public void fixBorderY(Terrain t)
    {
        /// get this Terrain's height data
        TerrainData data = GetComponent<Terrain>().terrainData;
        float[,] heights = data.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        /// get the other Terrain's height data
        TerrainData data2 = t.terrainData;
        float[,] heights2 = data2.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        /// set the terrain neighbors
        setRightTerrain(t);
        t.GetComponent<UTerrain>().setLeftTerrain(GetComponent<Terrain>());

        for (int i = 0; i < data.alphamapWidth; i++)
            heights[i, data.alphamapWidth - 1] = heights2[i, 0];

        /// Apply the border height changes to the terrain data and then update the terrain
        data.SetHeights(0, 0, heights);
        GetComponent<Terrain>().ApplyDelayedHeightmapModification();
    }

    /// <summary>
    /// Fixes the (diagonal) Northeastern edge
    /// </summary>
    /// <param name="t">Eastern (Right) terrain</param>
    public void fixEdge(Terrain t)
    {
        /// get this Terrain's height data
        TerrainData data = GetComponent<Terrain>().terrainData;
        float[,] heights = data.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        /// get the other Terrain's height data
        TerrainData data2 = t.terrainData;
        float[,] heights2 = data2.GetHeights(0, 0, data.alphamapWidth, data.alphamapWidth);

        heights[data.alphamapWidth - 1, data.alphamapWidth - 1] = heights2[0, 0];

        /// Apply the border height changes to the terrain data and then update the terrain
        data.SetHeights(0, 0, heights);
        GetComponent<Terrain>().ApplyDelayedHeightmapModification();
    }

    private void genHeight()
    {
        Terrain terrain = GetComponent<Terrain>();

        Texture2D data = new Texture2D(256, 256);

        byte[] rawData = new byte[256 * 256];
        rawData = loader.heightData;
        data.LoadImage(rawData);
        data.Apply();
        GetComponent<Transform>().position = new Vector3(transform.position.x, terrainDepth, transform.position.z);
        TerrainData td = terrain.terrainData;
        td.alphamapResolution = 257;
        td.heightmapResolution = 257;

        td.size = new Vector3(td.size.x, terrainHeight*10, td.size.z);

        float[,] heights = new float[td.alphamapWidth, td.alphamapWidth];

        for (int i = 0; i < td.alphamapWidth; i++)
        {
            for (int j = 0; j < td.alphamapWidth; j++)
            {
                Color c = data.GetPixel(i, j);
                float r = c.r;
                float g = c.g;
                float b = c.b;
                heights[j, i] = (r * 256 + g + b / 256) / 256;

                /// This will merge with the border of another terrain
                if (i == td.alphamapWidth - 1) heights[j, i] = heights[j, td.alphamapWidth - 2];
            }
            /// This will merge with the border of another terrain
            heights[td.alphamapWidth - 1, i] = heights[td.alphamapWidth - 2, i];
        }

        td.SetHeights(0, 0, heights);
        terrain.ApplyDelayedHeightmapModification();
    }

    private void Start()
    {
        neighbors = new Terrain[4];
        loader = new UTerrainLoader(tile);
        loader.Start();
    }

    private void Update()
    {
        /// Deletes the terrain if the reference to player or tile desapears
        /// this fixes problemes in editor when recompiling the code
        if (player == null || tile == null)
        {
            destroy();
        }

        /// Check if the thread has finished his work
        if(loader != null)
        {
            if (loader.Update())
            {
                /// Here the thread have finished
                genHeight();
                generated = true;
                loader = null;
            }
        }

        /// fullFixed    - all the fixes has been applied (this one is the first because 
        ///                is the most restrictive and there is no need to process the 
        ///                other comparasions)
        /// generated    - guarantee that all the data info from this terrain is fully loaded
        /// tile != null - fixes errors in editor when recompiling the code
        if (!fullFixed && generated && tile != null)
        {
            /// if X border is not fixed, fix it
            if (!borderXFixed)
            {
                Hashtable map = player.getMap();
                string topTerrainKey = UPlayer.genTerrainName(tile.x, tile.y-1);
                if (map.ContainsKey(topTerrainKey))
                {
                    GameObject terrain = (GameObject)map[topTerrainKey];
                    if (terrain.GetComponent<UTerrain>().generated)
                    {
                        fixBorderX(terrain.GetComponent<Terrain>());
                        borderXFixed = true;
                    }
                }
            }

            /// if Y border is not fixed, fix it
            if (!borderYFixed)
            {
                Hashtable map = player.getMap();
                string rightTerrainKey = UPlayer.genTerrainName(tile.x+1, tile.y);
                if (map.ContainsKey(rightTerrainKey))
                {
                    GameObject terrain = (GameObject)map[rightTerrainKey];
                    if (terrain.GetComponent<UTerrain>().generated)
                    {
                        fixBorderY(terrain.GetComponent<Terrain>());
                        borderYFixed = true;
                    }
                }
            }

            /// if the farest edge from (0,0) is not fixed, fix it
            if (!edgeFixed)
            {
                Hashtable map = player.getMap();
                string rightTerrainKey = UPlayer.genTerrainName(tile.x + 1, tile.y - 1);
                if (map.ContainsKey(rightTerrainKey))
                {
                    GameObject terrain = (GameObject)map[rightTerrainKey];
                    if (terrain.GetComponent<UTerrain>().generated)
                    {
                        fixEdge(terrain.GetComponent<Terrain>());
                        edgeFixed = true;
                    }
                }
            }
            
            if(borderXFixed && borderYFixed && edgeFixed)
            {
                GetComponent<TerrainCollider>().terrainData = GetComponent<Terrain>().terrainData;
                fullFixed = true;
                // TODO: throw a thread and save the height in disk.
            }
        }
    }
}
