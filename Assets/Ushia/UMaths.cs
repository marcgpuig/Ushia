using System;
using UnityEngine;

/// <summary>
/// Ushia Maths
/// </summary>
public static class UMaths
{
    /// Earth radius in meters on the equator
    public static double RADIUS = 6378137.0;

    public static double sec(double a) { return 1.0 / Math.Cos(a); }

    /// <summary>
    /// Degrees to radiants.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static double deg2Rad(double angle)
    {
        return (Math.PI * angle / 180.0);
    }

    /// <summary>
    /// Radiants to degrees.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static double rad2Deg(double angle)
    {
        return angle * (180.0 / Math.PI);
    }

    /// <summary>
    /// 
    /// More info: http://stackoverflow.com/questions/837872/calculate-distance-in-meters-when-you-know-longitude-and-latitude-in-java
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="lng1"></param>
    /// <param name="lat2"></param>
    /// <param name="lng2"></param>
    /// <returns></returns>
    public static float distFrom(float lat1, float lng1, float lat2, float lng2)
    {
        double earthRadius = 6371000; //meters
        double dLat = deg2Rad(lat2 - lat1);
        double dLng = deg2Rad(lng2 - lng1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(deg2Rad(lat1)) * Math.Cos(deg2Rad(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        float dist = (float)(earthRadius * c);
        return dist;
    }

    /// Spherical Pseudo-Mercator projection functions
    /// from OpenStreetMaps Wiki: http://wiki.openstreetmap.org/wiki/Mercator#C.23

    public static double lat2y(double lat)
    {
        return  Math.Log(Math.Tan(deg2Rad(lat) / 2 + Math.PI / 4)) * RADIUS;
    }

    /// my interpretation of lon functions
    public static double x2lon(double aX)
    {
        return rad2Deg(aX / RADIUS);
    }

    public static double lon2x(double aLong)
    {
        return deg2Rad(aLong) * RADIUS;
    }

    /// <summary>
    /// How many (integer) times is "floor" in "value"
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int scaledFloor(double floor, double value)
    {
        if (value < 0.0)
            value = value - floor;
        return (int)(value - (value % floor));
    }
}

public class Int3
{
    public int x, y, z;

    public Int3(int _x = 0, int _y = 0, int _z = 0)
    {
        set(_x, _y, _z);
    }

    public void set(int _x = 0, int _y = 0, int _z = 0)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public void set(Int3 i)
    {
        x = i.x;
        y = i.y;
        z = i.z;
    }

    public static Int3 findChunk(float _x, float _y, float _z, float chunkSize)
    {
        return new Int3((int)(UMaths.scaledFloor(chunkSize, _x) / chunkSize), (int)(UMaths.scaledFloor(chunkSize, _y) / chunkSize), (int)(UMaths.scaledFloor(chunkSize, _z) / chunkSize));
    }

    public static Int3 findChunk(Vector3 vec, float chunkSize)
    {
        return findChunk(vec.x, vec.y, vec.z, chunkSize);
    }

    public void set(Vector3 vec, float chunkSize)
    {
        set(findChunk(vec.x, vec.y, vec.z, chunkSize));
    }

    public void set(float _x, float _y, float _z, float chunkSize)
    {
        set(findChunk(_x, _y, _z, chunkSize));
    }

    public Int3 abs()
    {
        return new Int3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
    }

    public static Int3 operator +(Int3 a, Int3 b)
    {
        return new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Int3 operator -(Int3 a, Int3 b)
    {
        return new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public override string ToString()
    {
        return (string)("(" + x + ", " + y + ", " + z + ")");
    }
}