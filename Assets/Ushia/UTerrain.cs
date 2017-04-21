using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class UTerrain : MonoBehaviour {

    public static float terrainDepth = -4400;
    public static float terrainHeight = 8848;

    private Terrain terrain;

	void Start ()
    {
        terrain = GetComponent<Terrain>();
	}

    public void genHeight(USlippyTile tile)
    {
        terrain = GetComponent<Terrain>();

        Texture2D data = HeightLoader.getHeight(tile);
        data.Apply();

        GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, terrainDepth, GetComponent<Transform>().position.z);
        TerrainData td = terrain.terrainData;
        td.alphamapResolution = 257;
        td.heightmapResolution = 257;

        td.size = new Vector3(td.size.x, terrainHeight, td.size.z);

        float[,] heights = new float[td.alphamapWidth, td.alphamapHeight];

        for (int i = 0; i < td.alphamapWidth; i++)
        {
            for (int j = 0; j < td.alphamapHeight; j++)
            {
                Color c = data.GetPixel(i, j);
                float r = c.r;
                float g = c.g;
                float b = c.b;
                heights[(td.alphamapHeight - 1) - j, i] = (r * 256 + g + b / 256) / 256;

                /// bad border (?)
                if (i == 256) heights[(td.alphamapHeight - 1) - j, i] = heights[(td.alphamapHeight - 1) - j, 255];
            }
            /// bad border (?)
            heights[0, i] = heights[1, i];
        }

        td.SetHeights(0, 0, heights);
        terrain.ApplyDelayedHeightmapModification();
    }

    public void genHeight(double lon, double lat, int zoom)
    {
        GCS target = new GCS(lon, lat);
        Texture2D data = HeightLoader.getHeight(target, zoom); //lon, lat
        data.Apply();

        GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, terrainDepth, GetComponent<Transform>().position.z);
        TerrainData td = terrain.terrainData;
        td.alphamapResolution = 257;

        float[,] heights = new float[td.alphamapWidth, td.alphamapHeight];

        for (int i = 0; i < td.alphamapWidth; i++)
        {
            for (int j = 0; j < td.alphamapHeight; j++)
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
            heights[256, i] = heights[255, i];
        }

        td.SetHeights(0, 0, heights);
        terrain.ApplyDelayedHeightmapModification();
    }
}
