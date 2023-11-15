using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Core.GameEditor.AssetLoaders
{
    public static class VideoLoader
    {
        public static async void SetVideo(GameObject gameObject, string url)
        {

        }

        public static async Task<string> GetVideo (string url)
        { 
            //UnityWebRequest www = UnityWebRequest.Get("https://example.com/video.mp4");
            //yield return www.SendWebRequest();

            //if (www.isNetworkError || www.isHttpError)
            //{
            //    Debug.Log(www.error);
            //}
            //else
            //{
            //    File.WriteAllBytes("path/to/file", www.downloadHandler.data);
            //}
            return "";
        }
    }
}
