using System;

public static class UshiaMaths
{
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
}
