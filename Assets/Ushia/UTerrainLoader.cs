using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class UTerrainLoader : UThreadWrapper {

    /// Tile to load
    public USlippyTile tile;
    public byte[] heightData;

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

    /// Threaded task. DON'T use the Unity API here!
    protected override void ThreadFunction()
    {
        heightData = getByteHeight(tile);
    }

    /// This is executed by the Unity main thread when the job is finished
    protected override void OnFinished()
    {

    }

    /// <summary>
    /// Validate SSL certificates when using HttpWebRequest
    /// From: http://answers.unity3d.com/questions/792342/how-to-validate-ssl-certificates-when-using-httpwe.html
    /// </summary>
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
}
