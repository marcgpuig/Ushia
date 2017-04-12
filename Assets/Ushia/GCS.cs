/// <summary>
/// Geographic Coordinate System
/// <see cref="https://en.wikipedia.org/wiki/Geographic_coordinate_system"/>
/// </summary>
public class GCS
{
    /// <summary>
    /// Longitude on a point on Earth's surface is the angle east 
    /// or west of a reference meridian to another meridian that 
    /// passes through that point.
    /// </summary>
    public double lon;
    /// <summary>
    /// Latitude on a point on Earth's surface is the angle between 
    /// the equatorial plane and the straight line that passes through 
    /// that point and through (or close to) the center of the Earth.
    /// </summary>
    public double lat;

    public GCS()
    {
        lon = 0;
        lat = 0;
    }

    public GCS(double _lon, double _lat)
    {
        lon = _lon;
        lat = _lat;
    }

    public void set(double _lon, double _lat)
    {
        lon = _lon;
        lat = _lat;
    }

    public GCS copy() { return new GCS(lon, lat); }

    public override string ToString() { return "(" + lon + ", " + lat + ")"; }

    public static GCS operator + (GCS g1, GCS g2) { return new GCS(g1.lon + g2.lon, g1.lat + g2.lat); }
    public static GCS operator - (GCS g1, GCS g2) { return new GCS(g1.lon - g2.lon, g1.lat - g2.lat); }
}
