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
            ServicePointManager.ServerCertificateValidationCallback = UUtils.MyRemoteCertificateValidationCallback;
            string url = OSMApiCall + "11.54,48.14,11.543,48.145";
            xml.LoadXml(client.DownloadString(url));
        }
        return xml;
    }
}
