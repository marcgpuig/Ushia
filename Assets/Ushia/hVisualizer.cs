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

        if (GetComponent<Terrain>() == null) return;

        float eMinH = -11000;
        float eMaxH = 9000;
        float normWaterLvl = (0 - eMinH) / (eMaxH - eMinH);
        float hDiff = eMaxH - eMinH;
        float constY = hDiff * normWaterLvl;

        GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, -4400, GetComponent<Transform>().position.z);
        TerrainData td = GetComponent<Terrain>().terrainData;
        td.heightmapResolution = 257;

        float[,] heights = new float[256,256];

        for (int i = 0; i < data.width; i++)
        {
            for (int j = 0; j < data.height; j++)
            {
                Color c = data.GetPixel(i, j);
                float r = c.r;
                float g = c.g;
                float b = c.b;
                //if (j % 100 == 0) Debug.Log(r + " " + g + " " + b);
                //heights[i, j] = ((r * 256 + g + b / 256) - 32768) * 0.05f;
                heights[j, i] = (r * 256 + g + b / 256) / 256;
                //if (j % 100 == 0) Debug.Log(heights[i, j]);
            }
        }

        td.SetHeights(0, 0, heights);
        GetComponent<Terrain>().ApplyDelayedHeightmapModification();
        //GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(td);
        //terrain.transform.position = new Vector3(lenght * (x - 1), 0, width * (y - 1));

        /// Tile that containts the given Longitude and Latitude
        GCS tile = HeightLoader.WorldToTilePos(target.lon, target.lat, zoom);
        tile.lon = (float)System.Math.Floor(tile.lon);
        tile.lat = (float)System.Math.Floor(tile.lat);

        /// Longitude and Latitude of the bottom left point of the tile
        GCS tileGCS = HeightLoader.TileToWorldPos(tile.lon, tile.lat, zoom);

        /// size of the terrain chunk in GCS
        GCS tileSize = HeightLoader.zoomSize(zoom);
        Debug.Log(tileSize.ToString());

        GCS diff = target - tileGCS;

        GCSImagePos = new Vector3();

        GCSImagePos.x = (float)UshiaMaths.lon2x(diff.lon);
        GCSImagePos.z = (float)UshiaMaths.lat2y(diff.lat);

        Debug.Log(GCSImagePos);

        int xx = (int)(GCSImagePos.x);
        int zz = (int)(GCSImagePos.z);

        GCSImagePos.y = td.GetHeight(xx, zz) + GetComponent<Transform>().position.y;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(GCSImagePos, 10);
    }
}
