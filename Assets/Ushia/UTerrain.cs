using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
public class UTerrain : MonoBehaviour {

    public static float terrainDepth = -4400;
    public static float terrainHeight = 8848;

    public bool generated = false;

    private Terrain terrain;
    private UTerrainLoader loader = null;

    public void init(USlippyTile tile)
    {
        terrain = GetComponent<Terrain>();
        loader = new UTerrainLoader();
        loader.tile = tile;
        loader.Start();
    }

    public void fixBorderX(Terrain t)
    {

    }

    private void Update()
    {
        if(loader != null)
        {
            if (loader.Update())
            {
                genHeight();
                generated = true;
                loader = null;
            }
        }
    }

    private void genHeight()
    {
        terrain = GetComponent<Terrain>();

        Texture2D data = new Texture2D(256, 256);
        //data.LoadImage(HeightLoader.getByteHeight(tile));

        byte[] rawData = new byte[256 * 256];
        rawData = loader.heightData;
        data.LoadImage(rawData);
        data.Apply();

        GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, terrainDepth, GetComponent<Transform>().position.z);
        TerrainData td = terrain.terrainData;
        td.alphamapResolution = 257;
        td.heightmapResolution = 257;

        td.size = new Vector3(td.size.x, terrainHeight, td.size.z);


        // TODO alphamapHeight is incorrect!!!!!! 
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

                /// bad border (?)
                if (i == 256) heights[j, i] = heights[j, 255];
            }
            /// bad border (?)
            heights[0, i] = heights[1, i];
        }

        td.SetHeights(0, 0, heights);
        terrain.ApplyDelayedHeightmapModification();
    }
}
