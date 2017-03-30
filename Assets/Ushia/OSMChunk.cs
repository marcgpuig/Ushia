using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OSMChunk
{
    OSMParser parser = null;

    /// <summary>
    /// bounds
    /// </summary>
    private double minLat = 0;
    private double minLon = 0;
    private double maxLat = 0;
    private double maxLon = 0;

    public Vector3 virtualWorldPos = Vector3.zero;

    public List<OSMNode> nodes = null;

    // geters
    public double getMinLat() { return minLat; }
    public double getMinLon() { return minLon; }
    public double getMaxLat() { return maxLat; }
    public double getMaxLon() { return maxLon; }

    public double getMinPosY() { return minLat; }
    public double getMinPosX() { return minLon; }
    public double getMaxPosY() { return maxLat; }
    public double getMaxPosX() { return maxLon; }

    // sets
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
        double minX = UshiaMaths.lon2x(minLon);
        double minY = UshiaMaths.lat2y(minLat);
        //Debug.Log(minX + " - " + minY);

        /// Mercator
        foreach (OSMNode n in nodes)
        {
            n.pos.x = (float)UshiaMaths.lon2x(n.lon) - (float)minX;
            n.pos.z = (float)UshiaMaths.lat2y(n.lat) - (float)minY;
        }

        return true;
    }
}
