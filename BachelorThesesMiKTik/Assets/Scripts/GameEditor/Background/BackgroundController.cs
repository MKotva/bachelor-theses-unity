using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] public GameObject LayerPrefab;
    [SerializeField] public List<GameObject> DefaultBackground;
    [SerializeField] public List<GameObject> BackgroundImages;

    private AssetLoader loader;

    private void Start()
    {
        loader = new AssetLoader();
    }


    public async Task SetImageAsync(string pathToSource, int layer)
    {
        var texture = await loader.LoadImageAsset(pathToSource);
        texture.Apply();

        AppendBackgroundLayer(texture, BackgroundImages.Count);
    }

    public async Task SetImagesAsync(List<string> images)
    {
        var texturesTasks = new List<Task<Texture2D>>();
        foreach (var image in images)
        {
            texturesTasks.Add(loader.LoadImageAsset(image));
        }

        await Task.WhenAll(texturesTasks);

        ClearBackground();
        for (int i = 0; i < texturesTasks.Count; i++)
        {
            var texture = texturesTasks[i].Result;
            texture.Apply(texture);

            AppendBackgroundLayer(texture, i + 1);
        }
    }

    private void AppendBackgroundLayer(Texture2D texture, int sortingLayerID)
    {
        GameObject layer = Instantiate(LayerPrefab, transform);
        var spriteRenderer = layer.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerID = -1 * sortingLayerID; //TODO: Fix orientation of camera, so the layer must not be negative.
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Scale(spriteRenderer);

        BackgroundImages.Add(layer);
    }

    public void ClearBackground()
    {
        foreach (GameObject background in BackgroundImages)
        {
            Destroy(background);
        }
        BackgroundImages.Clear();
    }

    private void Scale(SpriteRenderer spriteRenderer)
    {
        var width = spriteRenderer.sprite.bounds.size.x;
        var height = spriteRenderer.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        spriteRenderer.transform.localScale = new Vector3((float) worldScreenWidth / width, (float)worldScreenHeight / height, 1);
    }
}
