using System;
using System.Net;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public static class HeightLoader {

    /// in Mapzen documentation:
    /// https://mapzen.com/documentation/terrain-tiles/use-service/#get-started-with-mapzen-terrain-tiles
    /// Note: GeoTIFF format tiles are 512x512 sized so request the parent tile’s coordinate. For instance, if you’re looking for a zoom 14 tile then request the parent tile at zoom 13
    public static int width  = 256;
    public static int height = 256;
    public static int zoom   =  14;  // 0 - 18 see http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames

    /// <summary>
    /// Validate SSL certificates when using HttpWebRequest
    /// From: http://answers.unity3d.com/questions/792342/how-to-validate-ssl-certificates-when-using-httpwe.html
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    /// <param name="sslPolicyErrors"></param>
    /// <returns></returns>
    public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    /// <summary>
    /// Top left point position of the tile in the world.
    /// From: http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#C.23
    /// </summary>
    /// <param name="lon">longitude</param>
    /// <param name="lat">latitude</param>
    /// <param name="zoom">zoom [0-18]</param>
    /// <returns></returns>
    public static GCS WorldToTilePos(double lon, double lat, int zoom)
    {
        GCS p = new GCS();
        p.lon = (float)((lon + 180.0) / 360.0 * (1 << zoom));
        p.lat = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
            1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

        return p;
    }

    public static GCS TileToWorldPos(double tile_x, double tile_y, int zoom)
    {
        GCS p = new GCS();
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

        p.lon = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
        p.lat = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
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

    /// <summary>
    /// Returns an array with a heightmap on it
    /// </summary>
    /// <param name="lon">longitude</param>
    /// <param name="lat">latitude</param>
    /// <returns></returns>
    public static Texture2D getHeight(GCS target, int zoom)
    {
        Texture2D tex;

        using (WebClient client = new WebClient())
        {
            // Zoom levels (zxy):
            // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
            // World APIs:
            // http://a.tile.stamen.com/terrain-background/12/656/1582.png
            // https://tile.mapzen.com/mapzen/terrain/v1/terrarium/12/656/1582.png?api_key=mapzen-HgL87jY"

            /// Tile that containts the given Longitude and Latitude.
            GCS tile = WorldToTilePos(target.lon, target.lat, zoom);
            tile.lon = (float)Math.Floor(tile.lon);
            tile.lat = (float)Math.Floor(tile.lat);

            // TODO change tile.lon and tile.lat for chunk.x and chunk.z
            //Int3 chunk = mapZenChunk(target.lon, target.lat, zoom);

            string url = "https://tile.mapzen.com/mapzen/terrain/v1/terrarium/" + zoom.ToString() + "/" + tile.lon.ToString() + "/" + tile.lat.ToString() + ".png?api_key=" + UVariables.mapzenAPIKey;
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            byte[] pngData = client.DownloadData(url);

            tex = new Texture2D(256, 256);
            tex.LoadImage(pngData);
            tex.Apply();
        }

        return tex;
    }

    /// <summary>
    /// Returns a Texture2D with a heightmap on it
    /// </summary>
    /// <param name="tile">tile x, y and zoom</param>
    /// <returns></returns>
    public static Texture2D getHeight(USlippyTile tile)
    {
        Texture2D tex;

        using (WebClient client = new WebClient())
        {
            string url = tile.getMapzenURLTerranium(UVariables.mapzenAPIKey);
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            byte[] pngData = client.DownloadData(url);

            tex = new Texture2D(256, 256);
            tex.LoadImage(pngData);
            tex.Apply();
        }

        return tex;
    }

    /// <summary>
    /// Returns an array with a heightmap on it
    /// </summary>
    /// <param name="tile">tile x, y and zoom</param>
    /// <returns></returns>
    public static byte[] getByteHeight(USlippyTile tile)
    {
        byte[] pngData;
        using (WebClient client = new WebClient())
        {
            string url = tile.getMapzenURLTerranium(UVariables.mapzenAPIKey);
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            pngData = client.DownloadData(url);
        }
        return pngData;
    }
}
