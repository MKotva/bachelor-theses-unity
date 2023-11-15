using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class BackgroundController : MonoBehaviour
{
    [SerializeField] public GameObject LayerPrefab;
    [SerializeField] public List<GameObject> DefaultBackgroundPrefabs;

    public List<GameObject> Background { get; private set; }

    private void Start()
    {
        Background = new List<GameObject>();
        SetDefault();
    }

    public async Task SetBackground(List<SourceDTO> sources)
    {
        ClearBackground();
        var texturesTasks = new List<Task>();
        for (int i = 0; i < sources.Count; i++)
        {
            var layer = AppendBackgroundLayer(i);
            texturesTasks.Add(SetLayer(layer, sources[i]));
        }

        await Task.WhenAll(texturesTasks);
    }

    public void SetDefault()
    {
        foreach(var backgroundPrefab in DefaultBackgroundPrefabs)
        {
            Background.Add(Instantiate(backgroundPrefab, transform));
        }
    }

    public void ClearBackground()
    {
        foreach (GameObject background in Background)
        {
            Destroy(background);
        }
        Background.Clear();
    }
    private GameObject AppendBackgroundLayer(int sortingOrder)
    {
        GameObject layer = Instantiate(LayerPrefab, transform);
        var spriteRenderer = layer.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1 * sortingOrder; //TODO: Fix orientation of camera, so the layer must not be negative.
        Background.Add(layer);

        return layer;
    }

    private async Task SetLayer(GameObject layer, SourceDTO source)
    {
        switch (source.Type) 
        {
            case SourceType.Image:
                await SpriteLoader.SetSprite(layer, source.URL, 1920, 1080);
                break;
            case SourceType.Video: 
                break;
            case SourceType.Animation:
                await AnimationLoader.SetAnimation(layer, ( (AnimationSourceDTO) source ).AnimationData, 1920, 1080);
                break;
            default: 
                break;
        }
    }
}
