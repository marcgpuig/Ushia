using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OSMChunk
{
    OSMParser parser = null;
    
    /// bounds
    private double minLat = 0;
    private double minLon = 0;
    private double maxLat = 0;
    private double maxLon = 0;

    public Vector3 virtualWorldPos = Vector3.zero;
    public double width = 0, height = 0; // x and z unity coordinates

    /// Here is explained why to use Hashtable
    /// http://cc.davelozinski.com/c-sharp/fastest-collection-for-string-lookups
    public Hashtable nodes = null;
    public Hashtable ways = null;

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

    public void setBounds(double _minLat, double _minLon, double _maxLat, double _maxLon)
    {
        setMinLat(_minLat);
        setMinLon(_minLon);
        setMaxLat(_maxLat);
        setMaxLon(_maxLon);
    }

    public void calculateDimensions()
    {
        width = UMaths.lon2x(maxLon) - UMaths.lon2x(minLon);
        height = UMaths.lat2y(maxLat) - UMaths.lat2y(minLat);
    }

    // constructors
    public OSMChunk() { }
    public OSMChunk(string filePath) { loadOSM(filePath); }

    private bool isParserInitialized() { return parser != null; }

    private void loadBounds()
    {
        minLat = parser.getMinLat();
        minLon = parser.getMinLon();
        maxLat = parser.getMaxLat();
        maxLon = parser.getMaxLon();
        calculateDimensions();
    }

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

    public bool loadNodes()
    {
        if (!isParserInitialized()) return false;
        nodes = parser.getNodes();

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

    public bool loadWays()
    {
        if (!isParserInitialized()) return false;
        ways = parser.getWays();
        return true;
    }
}
