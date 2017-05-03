using System.Net;
using UnityEngine;

public class UTerrainLoader : UThreadWrapper
{
    /// Tile to load
    public USlippyTile tile;
    public byte[] heightData;

    public UTerrainLoader(USlippyTile t) { tile = t; }

    /// <summary>
    /// Returns an array with a heightmap on it
    /// </summary>
    /// <param name="tile">tile x, y and zoom</param>
    /// <returns></returns>
    public static byte[] getByteHeight(USlippyTile tile)
    {
        // ERROR HERE
        byte[] pngData;
        using (WebClient client = new WebClient())
        {
            string url = tile.getMapzenURLTerranium(UVariables.mapzenAPIKey);
            ServicePointManager.ServerCertificateValidationCallback = UUtils.MyRemoteCertificateValidationCallback;
            pngData = client.DownloadData(url);
        }
        return pngData;
    }

    /// Threaded task. DON'T use the Unity API here!
    protected override void ThreadFunction()
    {
        heightData = getByteHeight(tile);
    }

    /// This is executed by the Unity main thread when the job is finished
    protected override void OnFinished()
    {
        Debug.Log("Terrain Loader " + tile + " done in " + (endTime - startTime).TotalSeconds + " sec.");
    }
}
