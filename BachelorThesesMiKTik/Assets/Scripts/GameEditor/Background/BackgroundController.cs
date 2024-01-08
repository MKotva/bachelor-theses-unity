using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Background;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackgroundController : Singleton<BackgroundController>
{
    [SerializeField] public GameObject LayerPrefab;
    [SerializeField] public List<GameObject> DefaultBackgroundPrefabs;

    public List<BackgroundLayer> BackgroundLayers { get; private set; }

    #region PUBLIC
    /// <summary>
    /// For each layer info, the method will create background layer(in order from farest to closest) of source type(Image/Animation).
    /// This method will also call scaling method
    /// </summary>
    /// <param name="layerInfos"></param>
    /// <returns></returns>
    public async Task SetBackground(List<BackgroundLayerInfoDTO> layerInfos)
    {
        ClearBackground();
        var texturesTasks = new List<Task>();
        foreach(var layerInfo in layerInfos)
        {
            if(layerInfo.Source == null)
            {
                if (!TryAppendDefaultLayer(layerInfo.ID))
                {
                    InfoPanelController.Instance.ShowMessage("Setting of background failed! Invalid source or default layer id.", "Background");
                    return;
                }
            }
            var layer = AppendBackgroundLayer(layerInfo);
            texturesTasks.Add(SetLayer(layer, layerInfo.Source, layerInfo.XSize, layerInfo.YSize));
        }

        await Task.WhenAll(texturesTasks);
    }


    /// <summary>
    /// For each source, the method will create background layer(in order from farest to closest) of source type(Image/Animation).
    /// This method will also call scaling method
    /// </summary>
    /// <param name="sources"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    /// <returns></returns>
    public async Task SetBackground(List<SourceDTO> sources, float xSize = 1920, float ySize = 1080)
    {
        ClearBackground();
        var texturesTasks = new List<Task>();
        for (int i = 0; i < sources.Count; i++)
        {
            var layer = AppendBackgroundLayer(new BackgroundLayerInfoDTO(sources[i], xSize, ySize, i));
            texturesTasks.Add(SetLayer(layer, sources[i], xSize, ySize));
        }

        await Task.WhenAll(texturesTasks);
    }

    /// <summary>
    /// Append new layer to the front of background.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public void AppendLayer(SourceDTO source, float xSize = 1920, float ySize = 1080)
    {
        var layer = AppendBackgroundLayer(new BackgroundLayerInfoDTO(source, xSize, ySize, BackgroundLayers.Count));
        var task = SetLayer(layer, source, xSize, ySize);
    }

    /// <summary>
    /// Changes the specific layer of background and scales it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="layerId"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public void SetLayer(SourceDTO source, int layerId, float xSize = 1920, float ySize = 1080)
    {
        var task = SetLayer(BackgroundLayers[layerId].Instance, source, xSize, ySize);
    }

    /// <summary>
    /// Sets default background.
    /// </summary>
    public void SetDefault()
    {
        for(int i = 0; i < DefaultBackgroundPrefabs.Count; i++)
        {
            TryAppendDefaultLayer(i);
        }
    }

    /// <summary>
    /// Removes all background layers
    /// </summary>
    public void ClearBackground()
    {
        foreach (var layer in BackgroundLayers)
        {
            Destroy(layer.Instance);
        }
        BackgroundLayers.Clear();
    }

    public void RemoveLayer(int layerId)
    {
        Destroy(BackgroundLayers[layerId].Instance);
        BackgroundLayers.RemoveAt(layerId);
    }
    #endregion

    #region PRIVATE
    private void Start()
    {
        BackgroundLayers = new List<BackgroundLayer>();
        SetDefault();
    }

    /// <summary>
    /// Creates new background layer and sets the sorting order in which will be displayed.
    /// </summary>
    /// <param name="sortingOrder"></param>
    /// <returns></returns>
    private GameObject AppendBackgroundLayer(BackgroundLayerInfoDTO info)
    {
        GameObject layer = Instantiate(LayerPrefab, transform);
        var spriteRenderer = layer.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1 * info.ID; //TODO: Fix orientation of camera, so the layer must not be negative.
        BackgroundLayers.Add(new BackgroundLayer(layer, info));

        return layer;
    }

    /// <summary>
    /// Based on given index, default layer is inserted. If index is invalid, returs false.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool TryAppendDefaultLayer(int index)
    {
        if(index >= 0 && index < DefaultBackgroundPrefabs.Count)
        {
            var layerInfo = new BackgroundLayerInfoDTO(null, 1920, 1080, index);
            var layer = new BackgroundLayer(Instantiate(DefaultBackgroundPrefabs[index], transform), layerInfo);
            BackgroundLayers.Add(layer);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method will decide (based on source valueType) which Layer should be added(Image/Animation).
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="source"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    /// <returns></returns>
    private async Task SetLayer(GameObject layer, SourceDTO source, float xSize, float ySize)
    {
        switch (source.Type) 
        {
            case SourceType.Image:
                await SpriteLoader.SetSprite(layer, source.URL, xSize, ySize);
                break;
            case SourceType.Animation:
                await AnimationLoader.SetAnimation(layer, ( (AnimationSourceDTO) source ).AnimationData, xSize, ySize);
                break;
            default: 
                break;
        }
    }
    #endregion
}
