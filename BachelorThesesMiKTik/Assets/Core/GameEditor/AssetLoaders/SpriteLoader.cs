using System;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Core.GameEditor.AssetLoaders
{
    public static class SpriteLoader
    {
        public static async Task SetSprite(GameObject ob, string url, uint xSize = 0, uint ySize = 0)
        {
            if (ob == null || url == string.Empty)
                return;

            var spriteRenderer = ob.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = await LoadSprite(url);

            if (xSize > 0 && ySize > 0)
            {
                Scale(spriteRenderer, xSize, ySize);
            }
        }

        public static async Task<Sprite> LoadSprite(string url)
        {
            var texture = await LoadTexture(url);

            if (texture == null)
                return null; // TODO Exception handling;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        //TODO: Use task RUN.
        public static async Task<Texture2D> LoadTexture(string url)
        {
            if (!url.Contains('/'))
            {
                url = GetAssetPath(url);
            }

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                request.certificateHandler = new Certificator();
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Delay(10);

                if (request.result == UnityWebRequest.Result.ProtocolError ||
                    request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    //TODO: Throw an exception.
                    InfoPanelController.Instance.ShowMessage(request.error);
                    return null;
                }

                var texture = DownloadHandlerTexture.GetContent(request);
                return texture;
            }
        }

        private static string GetAssetPath(string relativePath)
        {
            return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
        }

        private static void Scale(SpriteRenderer spriteRenderer, uint xSize, uint ySize)
        {
            var rect = spriteRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            spriteRenderer.transform.localScale = new Vector3((float) xScale, (float) yScale, 1);
        }
    }
}
