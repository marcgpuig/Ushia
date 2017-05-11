using System;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Tiles are 256 × 256 pixel PNG files
/// Each zoom level is a directory, each column is a subdirectory, and each tile in that column is a file
/// Filename(url) format is /zoom/x/y.png
/// 
/// Reference:
/// http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
/// </summary>
public class USlippyTile
{
    /// X goes from 0 (left edge is 180 °W) to 2zoom − 1 (right edge is 180 °E)
    public int x;

    /// Y goes from 0 (top edge is 85.0511 °N) to 2zoom − 1 (bottom edge is 85.0511 °S) in a Mercator projection
    public int y;

    /// The zoom parameter is an integer between 0 (zoomed out) and 18 (zoomed in). 18 is normally the maximum, but some tile servers might go beyond that.
    public int zoom;

    public USlippyTile()
    {
        set(0, 0, 0);
    }

    public USlippyTile(int _x, int _y, int _z)
    {
        set(_x, _y, _z);
    }

    public USlippyTile(double _x, double _y, int _z)
    {
        set(_x, _y, _z);
    }

    public void set(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        zoom = _z;
    }

    public void set(double lon, double lat, int _zoom)
    {
        GCS p = new GCS(lon, lat);
        set(p, _zoom);
    }

    public void set(GCS p, int _zoom)
    {
        USlippyTile a = GCS2Slippy(p, _zoom);
        x = a.x;
        y = a.y;
        zoom = _zoom;
    }

    public double getTileSize()
    {
        return zoomSize(Slippy2GCS(this).lat, zoom);
    }

    /// <summary>
    /// Ground resolution per zoom in meters at a given latitude.
    /// From: https://mapzen.com/documentation/terrain-tiles/data-sources/
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="zoom">zoom level</param>
    /// <returns></returns>
    public static double zoomSize(double lat, int zoom)
    {
        double r = Math.PI / 180;
        double t = lat * r;
        double a = Math.Cos(t) * 2 * Math.PI * 6378137;
        double b = 256 * Math.Pow(2, zoom);
        return a / b;
    }

    public static GCS Slippy2GCS(USlippyTile tile)
    {
        GCS p = new GCS();
        double n = Math.PI - ((2.0 * Math.PI * tile.y) / Math.Pow(2.0, tile.zoom));

        p.lon = (float)((tile.x / Math.Pow(2.0, tile.zoom) * 360.0) - 180.0);
        p.lat = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    public GCS getGCS()
    {
        return Slippy2GCS(this);
    }

    /// <summary>
    /// Calculates the north-east (top-left) GCS minLat and maxLat 
    /// to get the maxLat and maxLon of the current chunk
    /// </summary>
    public GCS getNorthEastGCS()
    {
        USlippyTile result = new USlippyTile(x, y, zoom);
        result.x += 1;
        result.y -= 1;
        if (result.x < 0 || result.y < 0)
            return null;
        return result.getGCS();
    }

    public static USlippyTile GCS2Slippy(GCS p, int _zoom)
    {
        p = HeightLoader.WorldToTilePos(p.lon, p.lat, _zoom);
        return new USlippyTile((int)System.Math.Floor(p.lon), (int)System.Math.Floor(p.lat), _zoom);
    }

    public static USlippyTile operator +(USlippyTile a, USlippyTile b)
    {
        if (a.zoom == b.zoom)
            return new USlippyTile(a.x + b.x, a.y + b.y, a.zoom);
        return null;
    }

    public static USlippyTile operator -(USlippyTile a, USlippyTile b)
    {
        if(a.zoom == b.zoom)
            return new USlippyTile(a.x - b.x, a.y - b.y, a.zoom);
        return null;
    }

    public override string ToString()
    {
        return (string)("(" + x + ", " + y + ", " + zoom + ")");
    }

    public string getMapzenURLTerranium(string apiKey)
    {
        return "http://tile.mapzen.com/mapzen/terrain/v1/terrarium/" + zoom.ToString() + "/" + x.ToString() + "/" + y.ToString() + ".png?api_key=" + apiKey;
    }

    public string getMapzenURLNormal(string apiKey)
    {
        return "http://tile.mapzen.com/mapzen/terrain/v1/normal/" + zoom.ToString() + "/" + x.ToString() + "/" + y.ToString() + ".png?api_key=" + apiKey;
    }

    public string getMapzenURLGeotiff(string apiKey)
    {
        return "http://tile.mapzen.com/mapzen/terrain/v1/geotiff/" + zoom.ToString() + "/" + x.ToString() + "/" + y.ToString() + ".png?api_key=" + apiKey;
    }
}