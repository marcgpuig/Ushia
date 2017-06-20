using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class OSMChunk : MonoBehaviour
{
    private OSMParser parser = null;
    public USlippyTile tile  = null;
    private UPlayer player;

    private bool downloaded = false;

    public bool isLoaded { get { return downloaded; } }
    public bool haveParser { get { return parser != null; } }
    /// bounds
    private double minLat = 0;
    private double minLon = 0;
    private double maxLat = 0;
    private double maxLon = 0;
    
    public double width = 0, height = 0; // x and z unity coordinates

    /// here is explained why to use Hashtable
    /// http://cc.davelozinski.com/c-sharp/fastest-collection-for-string-lookups
    public Hashtable nodes = null;
    public Hashtable nodesOutOfChunk = null;
    public Hashtable ways  = null;

    private bool nodesSettedInTerrain = false;

    /// geters
    public double getMinLat() { return minLat; }
    public double getMinLon() { return minLon; }
    public double getMaxLat() { return maxLat; }
    public double getMaxLon() { return maxLon; }

    public double getMinPosY() { return minLat; }
    public double getMinPosX() { return minLon; }
    public double getMaxPosY() { return maxLat; }
    public double getMaxPosX() { return maxLon; }

    /// sets
    public void setMinLat(double _minLat) { minLat = _minLat; }
    public void setMinLon(double _minLon) { minLon = _minLon; }
    public void setMaxLat(double _maxLat) { maxLat = _maxLat; }
    public void setMaxLon(double _maxLon) { maxLon = _maxLon; }

    public void init(USlippyTile t, UPlayer p)
    {
        tile = t;
        player = p;
        parser = new OSMParser(tile);
        parser.Start();
    }

    public void setBounds(double _minLat, double _minLon, double _maxLat, double _maxLon)
    {
        setMinLat(_minLat);
        setMinLon(_minLon);
        setMaxLat(_maxLat);
        setMaxLon(_maxLon);
    }

    private void loadBounds()
    {
        minLat = parser.bounds[0];
        minLon = parser.bounds[1];
        maxLat = parser.bounds[2];
        maxLon = parser.bounds[3];
        width = parser.width;
        height = parser.height;
    }

    /*
    public bool loadOSM(string filePath)
    {
        // load OSM data
        string rawData = OSMLoader.loadFromFile(filePath);
        if (rawData == "") return false;

        // initialize the parser
        parser = new OSMParser();
        parser.load(rawData);
        loadBounds();

        return true;
    }
    */

    private bool loadNodes()
    {
        nodes = parser.nodes;

        /// normalize
        double minX = UMaths.lon2x(minLon);
        double minY = UMaths.lat2y(minLat);

        /// Mercator
        foreach (DictionaryEntry e in nodes)
        {
            OSMNode n = (OSMNode)e.Value;
            n.pos.x = (float)UMaths.lon2x(n.lon) - (float)minX;
            n.pos.z = (float)UMaths.lat2y(n.lat) - (float)minY;
        }
        return true;
    }

    private bool loadWays()
    {
        ways = parser.ways;
        return true;
    }

    private void Update()
    {
        /// check if the thread has finished his work
        if (parser != null)
        {
            if (parser.Update())
            {
                /// here the thread have finished
                /// lets get the data from the parser
                loadBounds();
                loadNodes();
                loadWays();

                /// set the parser to null to ignore it one time has finished
                parser = null;
                downloaded = true;
            }
        }

        if (parser == null && nodes != null && GetComponent<UTerrain>() != null && GetComponent<UTerrain>().isFullyLoaded && !nodesSettedInTerrain)
        {
            foreach (DictionaryEntry e in nodes)
            {
                OSMNode n = (OSMNode)e.Value;
                n.pos.y = GetComponent<Terrain>().SampleHeight(n.pos + transform.position) + GetComponent<Terrain>().GetPosition().y;
            }
            
            nodesSettedInTerrain = true;
        }
    }
}
