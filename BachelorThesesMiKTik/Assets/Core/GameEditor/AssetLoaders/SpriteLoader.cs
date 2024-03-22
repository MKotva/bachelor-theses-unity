using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Core.GameEditor.AssetLoaders
{
    public static class SpriteLoader
    {
        /// <summary>
        /// Loads a sprite via LoadSprite() and sets the size. If the size is not given, method will
        /// scale sprite to fit a size of an object. Sprite will be assigned to a given object.
        /// </summary>
        /// <param name="ob">Object for set.</param>
        /// <param name="url">Path to source.</param>
        /// <param name="xSize">X scale size.</param>
        /// <param name="ySize">Y scale size.</param>
        /// <returns></returns>
        public static async Task SetSprite(GameObject ob, string url, float xSize = 0, float ySize = 0)
        {
            if (ob == null || url == string.Empty)
            {
                ErrorOutputManager.Instance.ShowMessage($"Unable to set sprite to an object! Object or url {url} is empty!");
                return;
            }

            if(xSize == 0 || ySize == 0)
            {
                if (ob.TryGetComponent(out RectTransform rect))
                {
                    xSize = rect.rect.width;
                    ySize = rect.rect.height;
                }
                else
                {
                    ErrorOutputManager.Instance.ShowMessage("Unable to set sprite to an object! Scale size is equal to Zero");
                    return;
                }
            }

            if(ob.TryGetComponent(out SpriteRenderer spriteRenderer)) 
            {
                await SetSprite(spriteRenderer, ob, url, xSize, ySize);
            }
            else if(ob.TryGetComponent(out Image image))
            {
                await SetSprite(image, ob, url, xSize, ySize);
            }
            else
            {
                ErrorOutputManager.Instance.ShowMessage("Unable to set sprite to an object! Object does not contain sprite renderer.");
            }
        }

        /// <summary>
        /// Loads a texture with LoadTextureMethod and than creates a new sprite.
        /// </summary>
        /// <param name="url">Path to texture.</param>
        /// <returns></returns>
        public static async Task<Sprite> LoadSprite(string url)
        {
            var texture = await LoadTexture(url);

            if (texture == null)
                return null; // TODO Exception handling;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        
        /// <summary>
        /// Loads texture from a given url. If the url does not contains /, method will try obtain the file from local folder (Streaming assets).
        /// </summary>
        /// <param name="url">Path to source</param>
        /// <returns></returns>
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
                    ErrorOutputManager.Instance.ShowMessage(request.error);
                    return null;
                }

                var texture = DownloadHandlerTexture.GetContent(request);
                return texture;
            }
        }

        /// <summary>
        /// Creates path to streaming asset folder.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private static string GetAssetPath(string relativePath)
        {
            return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
        }

        private static async Task SetSprite(SpriteRenderer renderer, GameObject ob, string url, float xSize = 0, float ySize = 0)
        {
            var sprite = await LoadSprite(url);
            if (sprite != null)
            {
                renderer.sprite = sprite;
                SetScale(renderer, xSize, ySize);
            }
        }

        private static async Task SetSprite(Image image, GameObject ob, string url, float xSize = 0, float ySize = 0)
        {
            var sprite = await LoadSprite(url);
            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }

        /// <summary>
        /// Set sprite renderer to a given size;
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        public static void SetScale(SpriteRenderer spriteRenderer, float xSize, float ySize)
        {
            var rect = spriteRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            spriteRenderer.transform.localScale = new Vector3(xScale, yScale, 1);
        }
    }
}
