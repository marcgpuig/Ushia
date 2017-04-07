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
    public static int zoom =    14;  // 0 - 18 see http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames

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

    //public class DPoint2 { public double x, y; }

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

    public static double TileToWorldPosX(double tile_x, int zoom)
    {
        return ((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
    }

    public static double TileToWorldPosY(double tile_y, int zoom)
    {
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));
        return (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));
    }

    public static GCS zoomSize(int zoom)
    {
        double pow = Math.Pow(2, zoom);
        return new GCS(360.0 / pow, 180.0 / pow);
    }

    /// <summary>
    /// returns an array with a heightmap on it
    /// </summary>
    /// <param name="lon">longitude</param>
    /// <param name="lat">latitude</param>
    /// <returns></returns>
    public static Texture2D getHeight(GCS target, int zoom)
    {
        double [] list = new double[width * height];
        Texture2D tex;
        using (WebClient client = new WebClient())
        {
            // Zoom levels (zxy):
            // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
            // World APIs:
            // http://a.tile.stamen.com/terrain-background/12/656/1582.png
            // https://tile.mapzen.com/mapzen/terrain/v1/terrarium/12/656/1582.png?api_key=mapzen-HgL87jY"

            /// Tile that containts the given Longitude and Latitude
            GCS tile = WorldToTilePos(target.lon, target.lat, zoom);
            tile.lon = (float)Math.Floor(tile.lon);
            tile.lat = (float)Math.Floor(tile.lat);

            string url = "https://tile.mapzen.com/mapzen/terrain/v1/terrarium/" + zoom.ToString() + "/" + tile.lon.ToString() + "/" + tile.lat.ToString() + ".png?api_key=mapzen-HgL87jY";
            //string url = "https://tile.mapzen.com/mapzen/terrain/v1/normal/" + zoom.ToString() + "/" + tiles.x.ToString() + "/" + tiles.y.ToString() + ".png?api_key=mapzen-HgL87jY";
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            byte[] pngData = client.DownloadData(url);

            tex = new Texture2D(256, 256);
            tex.LoadImage(pngData);
            tex.Apply();

            //client.DownloadFile(new Uri(url), @"c:\temp\image35.png");
            //OR 
            //client.DownloadFileAsync(new System.Uri(url), @"c:\temp\image35.png");

            //byte[] data = tex.GetRawTextureData();

            /*for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Debug.Log(i * height + j);

                    //byte r = data[i * height * 3 + j + 0];
                    //byte g = data[i * height * 3 + j + 1];
                    //byte b = data[i * height * 3 + j + 2];

                    Color c = tex.GetPixel(i, j);

                    /// decode:
                    /// h = (red * 256 + green + blue / 256) - 32768
                    list[i * width + j] = (c.r * 256 + c.g + c.b / 256) - 32768;
                }
            }*/
        }
        return tex;
    }
}
