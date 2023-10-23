using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;

public class AssetLoader
{
    //TODO: Use task RUN.
    public async Task<Texture2D> LoadImageAsset(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(GetAssetPath(url)))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Delay(10);

            if (request.isNetworkError || request.isHttpError || request.isError)
            {
                //TODO: Throw an exception.
                InfoPanelController.Instance.ShowMessage(request.error);
                return null;
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            return texture;
        }
    }

    private string GetAssetPath(string relativePath)
    {
        relativePath = "image.jpg";
        return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
    }


}
