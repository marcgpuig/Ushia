using System;

public static class UshiaMaths
{
    /// Earth radius in meters on the equator
    public static double RADIUS = 6378137.0;

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
}
