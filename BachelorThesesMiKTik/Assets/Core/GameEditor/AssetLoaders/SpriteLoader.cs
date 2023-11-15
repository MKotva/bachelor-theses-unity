using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Core.GameEditor.AssetLoaders
{
    public static class SpriteLoader
    {
        public static async Task SetSprite(GameObject ob, string url, uint xSize, uint ySize)
        {
            if (ob == null || url == string.Empty)
                return;

            var spriteRenderer = ob.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = await LoadSprite(url);
            Scale(spriteRenderer, xSize, ySize);
        }

        public static async Task<Sprite> LoadSprite(string url)
        {
            var texture =  await LoadTexture(url);
            
            if(texture == null)
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
            var width = spriteRenderer.sprite.bounds.size.x;
            var height = spriteRenderer.sprite.bounds.size.y;

            var worldScreenHeight = Camera.main.orthographicSize * 2.0;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            spriteRenderer.transform.localScale = new Vector3((float) worldScreenWidth / width, (float) worldScreenHeight / height, 1);
        }
    }
}
