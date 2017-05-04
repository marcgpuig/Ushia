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
            GCS gcs = USlippyTile.Slippy2GCS(tile);
            ServicePointManager.ServerCertificateValidationCallback = UUtils.MyRemoteCertificateValidationCallback;
            //               # RightLon  #LowLat   #LeftLon  #HiLat
            //chunkLimits = [2.13078,    41.48236, 2.13454,  41.48593]
            string url = OSMApiCall + gcs.lon + "," + gcs.lat + "," + (gcs.lon + 0.005) + "," + (gcs.lat + 0.005);
            xml.LoadXml(client.DownloadString(url));
        }
        return xml;
    }
}
