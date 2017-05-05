using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class ImageDownloader : MonoBehaviour
{
    public string url;
    public bool load = false;

    public byte[] downloadData(string url)
    {
        byte[] data;
        using (WebClient client = new WebClient())
        {
            Debug.Log(url);
            data = client.DownloadData(url);
        }
        return data;
    }

    public void applyTexture()
    {
        Texture2D text = new Texture2D(256, 256);
        text.LoadImage(downloadData(url));
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", text);
    }

    void Update ()
    {
        if(load)
        {
            applyTexture();
            load = false;
        }
    }
}
