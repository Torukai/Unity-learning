using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using ImageSplitter;

public class FileImporter : MonoBehaviour
{
    string path;
    public RawImage image;
    
    public void OpenExplorer()
	{
        path = EditorUtility.OpenFilePanel("Open image png", "", "jpg");
        GetImage();

    }

    void GetImage()
	{
        if (path != null)
		{
            UpdateImage();
            

        }
	}

    void UpdateImage()
	{

        WWW www = new WWW("file:///" + path);
        image.texture = www.texture;
        int w = www.texture.width;
        int h = www.texture.height;
        Texture2D aText = new Texture2D(2,2);
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(w/2, h/2);
        bool a = aText.LoadImage(www.bytes);
        Debug.Log(a);
        DrawImage(aText);// ((Texture2D)image.texture);


        //UnityWebRequest www = UnityWebRequest.Get("file:///" + path);
        //Texture2D tex = new Texture2D(300, 300);

        //tex.LoadImage(www.downloadHandler.data);
        //image.texture = tex;
    }

	void DrawImage(Texture2D sourceTexture)
	{
        Texture2D graph;
        Texture2D grayImg;
        graph = sourceTexture;//thisTexture is ARGB32 renders correctly

        //convert texture
        grayImg = new Texture2D(graph.width, graph.height, graph.format, false);
        Graphics.CopyTexture(graph, grayImg);
        Color32[] pixels = grayImg.GetPixels32();
        Color32[] changedPixels = new Color32[grayImg.width * grayImg.height];
        NativeArray<byte> bytes = graph.GetRawTextureData<byte>();

        for (int i = 0; i < bytes.Length; i += 4)
        {
            byte gray = (byte)(0.2126f * bytes[i + 1] + 0.7152f * bytes[i + 2] + 0.0722f * bytes[i + 3]);
            bytes[i + 3] = bytes[i + 2] = bytes[i + 1] = gray;
        }
        graph.Apply();
        image.texture = graph;
        //grayImg.SetPixels32(changedPixels);
        //grayImg.Apply(false);
    }

    void SplitImage()
	{

	}
}
