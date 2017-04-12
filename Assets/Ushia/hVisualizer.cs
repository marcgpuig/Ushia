using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class hVisualizer : MonoBehaviour
{
    public Material m;
    public Texture2D data;
    //public Vector2 GCSTarget = Vector2.zero;
    public double lon = 0;
    public double lat = 0;
    [Range(0, 18)]
    public int zoom = 12;
    public bool update = false;
    Terrain terrain = null;
    private Vector3 GCSImagePos; /// Geographic coordinate system

    public void genHeight()
    {
        GCS target = new GCS(lon, lat);
        data = HeightLoader.getHeight(target, zoom); //lon, lat

        m = new Material(Shader.Find("Unlit/Texture"));
        if(GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().material = m;
        data.Apply();
        m.SetTexture("_MainTex", data);

        GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -4400, GetComponent<Transform>().position.z);
        TerrainData td = terrain.terrainData;
        //td.heightmapResolution = 257;
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
                //if (j % 100 == 0) Debug.Log(r + " " + g + " " + b);
                //heights[i, j] = ((r * 256 + g + b / 256) - 32768) * 0.05f;
                //if (j % 100 == 0) Debug.Log(heights[i, j]);
                heights[j, i] = (r * 256 + g + b / 256) / 256;

                /// bad border (?)
                if (i == 256) heights[j, i] = heights[j, 255];
            }
            /// bad border (?)
            heights[256, i] = heights[255, i];
        }

        td.SetHeights(0, 0, heights);
        terrain.ApplyDelayedHeightmapModification();

        GCSImagePos = new Vector3();
        updatePointPosition();
    }

	void Start ()
    {
        genHeight();
    }
	
	void Update ()
    {
        if (update)
        {
            update = false;
            genHeight();
        }
	}

    void updatePointPosition()
    {
        /// Top left point position of the tile in the world.
        GCS tile = HeightLoader.WorldToTilePos(lon, lat, zoom);

        /// Get the decimals in the interval [0 - 1)
        tile.lon = tile.lon - System.Math.Truncate(tile.lon);
        tile.lat = tile.lat - System.Math.Truncate(tile.lat);

        /// Terrain coordinates in Unity starts in bottom left, BUT!
        /// terrain coordinates in Height map starts in top left
        GCSImagePos.x =       (float)(tile.lon * 256);
        GCSImagePos.z = 256 - (float)(tile.lat * 256); /// se we need to subtract that
        
        /// Coordinates in height map
        int xx =       (int)(tile.lon * 256);
        int zz = 256 - (int)(tile.lat * 256);

        /// Update position
        GCSImagePos.y = terrain.terrainData.GetHeight(xx, zz) + GetComponent<Transform>().position.y;
    }

    private void OnDrawGizmos()
    {
        updatePointPosition();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GCSImagePos, GCSImagePos + new Vector3(0,100,0));
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(GCSImagePos, 1f);
        Gizmos.DrawIcon(GCSImagePos, "UshiaLocation.png", true);
    }
}
