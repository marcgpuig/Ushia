using System.Xml;
using System.Net;

public static class OSMDownloader
{
    private static string OSMApiCall = "http://api.openstreetmap.org/api/0.6/map?bbox=";

    /// <summary>
    /// Uses the OpenStreetMap API to get the desired data in a latitude and longitude bounds.
    /// </summary>
    /// <param name="tile">tile x, y and zoom</param>
    /// <returns>An XML with the OSM data</returns>
    public static XmlDocument getOSMXML(USlippyTile tile)
    {
        XmlDocument xml = new XmlDocument();
        using (WebClient client = new WebClient())
        {
            //tile.y
            USlippyTile copy = new USlippyTile(tile.x, tile.y, tile.zoom);
            copy.y += 1;
            GCS gcs = copy.getGCS();
            /// North-East USlippyTile's GCS
            GCS gcsNE = copy.getNorthEastGCS();
            //              # LefttLon #LowLat   #RightLon #HiLat
            //chunkLimits = [2.13078,  41.48236, 2.13454,  41.48593]
            string url = OSMApiCall + gcs.lon + "," + gcs.lat + "," + gcsNE.lon + "," + gcsNE.lat;
            xml.LoadXml(client.DownloadString(url));
        }
        return xml;
    }
}
