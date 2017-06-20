using UnityEngine;
using System.Collections;
using System.Linq; // used for Sum of array

// https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/

// calculate normals stepness in another thread:
// http://answers.unity3d.com/questions/1123848/calculate-gradient-steepness-of-a-terrain-point-ma.html

[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
public class AssignSplatMap : MonoBehaviour
{
    public bool update = false;

    public int erosionSteps   = 0;
    public float erosionPower = 0.1f;

    void Start()
    {
        // Get the attached terrain component
        Terrain terrain = GetComponent<Terrain>();

        SplatPrototype[] textures = new SplatPrototype[4];

        textures[0].texture = (Texture2D)Resources.Load("Materials/terrain/03/adesert_rocky_d");
        textures[0].normalMap = (Texture2D)Resources.Load("Materials/terrain/03/desert_rocky_n");

        textures[1].texture = (Texture2D)Resources.Load("Materials/terrain/03/grass_ground2y_d");
        textures[1].normalMap = (Texture2D)Resources.Load("Materials/terrain/03/grass_ground_n");

        textures[2].texture = (Texture2D)Resources.Load("Materials/terrain/03/mntn_white_d");
        textures[2].normalMap = (Texture2D)Resources.Load("Materials/terrain/03/jungle_stone_n");

        textures[3].texture = (Texture2D)Resources.Load("Materials/terrain/03/snow_bumpy_d");
        textures[3].normalMap = (Texture2D)Resources.Load("Materials/terrain/03/snow_bumpy_n");

        terrain.terrainData.splatPrototypes = textures;
    }

    private float genHeight(float actualHeight, float intervalMin, float intervalMax)
    {
        if (actualHeight > intervalMax || actualHeight < intervalMin) return 0.0f;
        float intervalHalf = intervalMin + ((intervalMax - intervalMin) / 2.0f);
        float t = intervalMax - intervalHalf;
        float r = Mathf.Abs(actualHeight - intervalHalf);
        return t / r;
    }

    public void repaint()
    {
        // Get the attached terrain component
        Terrain terrain = GetComponent<Terrain>();

        // Get a reference to the terrain data
        TerrainData terrainData = terrain.terrainData;

        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Height [0.0 - 1.0]
                float nheight = height / terrainData.heightmapScale[1];

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                /*// Texture[0] has constant influence
                splatWeights[0] = 0.0f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);*/


                //splatWeights[0] = Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 0.05f));
                //splatWeights[1] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 0.5f));
                //Mathf.Lerp();
                float flatness = Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 0.1f));
                splatWeights[2] = flatness;

                splatWeights[0] = (genHeight(nheight, 0.33f, 0.76f)) * (1.0f - flatness);
                splatWeights[1] = (genHeight(nheight, 0.0f, 0.43f)) * (1.0f - flatness);
                splatWeights[3] = (genHeight(nheight, 0.56f, 1.0f)) * (1.0f - flatness);

                //splatWeights[3] = 1.0f - Mathf.Clamp01(height * height / (terrain.drawHeightmap / 1.0f));
                //splatWeights[3] = Mathf.Clamp01(height * height / (terrainData.heightmapHeight / 1.0f));

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();
                
                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }
        /*
        // erosion
        float[,] sediment = new float[terrainData.alphamapWidth, terrainData.alphamapHeight];
        for (int i = 0; i < erosionSteps; i++)
        {
            for (int y = 1; y < terrainData.alphamapHeight - 1; y++)
            {
                for (int x = 1; x < terrainData.alphamapWidth - 1; x++)
                {
                    float y_01 = (float)y / (float)terrainData.alphamapHeight;
                    float x_01 = (float)x / (float)terrainData.alphamapWidth;

                    // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                    float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                    float splatWeights = splatmapData[x, y, 0];
                    
                    if(splatWeights > 0)
                    {
                        int fi = 0, fj = 0;
                        float min = float.MaxValue;
                        for (int ii = -1; ii < 3; ii++)
                        {
                            for (int jj = -1; jj < 3; jj++)
                            {
                                float y_01_ = ((float)y+ii) / (float)terrainData.alphamapHeight;
                                float x_01_ = ((float)x+jj) / (float)terrainData.alphamapWidth;
                                float current = terrainData.GetHeight(Mathf.RoundToInt((y_01_) * terrainData.heightmapHeight), Mathf.RoundToInt((x_01_) * terrainData.heightmapWidth));
                                if (current < min)
                                {
                                    min = current;
                                    fi = ii;
                                    fj = jj;
                                }
                            }
                        }

                        if (fj != 0 && fi != 0)
                        {
                            splatmapData[x + fj, y + fi, 0] += Mathf.Clamp01(splatmapData[x, y, 0] * erosionPower);
                            splatmapData[x + fj, y + fi, 0] = Mathf.Clamp01(splatmapData[x + fj, y + fi, 0]);
                        }
                    }
                }
            }
        }
        */
        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private void Update()
    {
        if(update)
        {
            repaint();
            update = false;
        }
    }
}