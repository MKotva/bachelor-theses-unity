using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackgroundController : Singleton<BackgroundController>
{
    [SerializeField] GameObject LayerPrefab;
    [SerializeField] public TMP_Dropdown AudioDropDown;
    [SerializeField] List<GameObject> DefaultBackgroundPrefabs;
    [SerializeField] public AudioController AudioController;

    public List<GameObject> BackgroundLayers { get; private set; }
    public List<SourceDTO> Sources { get; private set; }
    public SourceDTO AudioSource { get; private set; }

    #region PUBLIC
    public void SetBackground(List<SourceDTO> sources)
    {
        if (sources.Count == 1)
        {
            if (sources[0].Name == null)
            {
                SetDefault();
                return;
            }
        }

        ClearBackground();
        foreach (var source in sources)
        {
            AppendLayer(source);
        }
    }

    /// <summary>
    /// Append new layer to the front of background.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public void AppendLayer(SourceDTO source)
    {
        var layer = AppendBackgroundLayer();
        SetLayer(layer, source);
        Sources.Add(source);
    }

    /// <summary>
    /// Changes the specific layer of background and scales it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="layerId"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public void SetLayer(SourceDTO source, int layerId)
    {
        SetLayer(BackgroundLayers[layerId], source);
        Sources[layerId] = source;
    }

    /// <summary>
    /// Sets default background.
    /// </summary>
    public void SetDefault()
    {
        ClearBackground();
        foreach (var defaultLayer in DefaultBackgroundPrefabs)
        {
            BackgroundLayers.Add(Instantiate(defaultLayer, transform));
        }
        Sources.Add(new SourceDTO(null, SourceType.Image));
    }

    /// <summary>
    /// Removes all background layers
    /// </summary>
    public void ClearBackground()
    {
        foreach (var layer in BackgroundLayers)
        {
            Destroy(layer);
        }
        BackgroundLayers.Clear();
        Sources.Clear();
    }

    public void RemoveLayer(int layerId)
    {
        Destroy(BackgroundLayers[layerId]);
        BackgroundLayers.RemoveAt(layerId);
        Sources.RemoveAt(layerId);
    }

    /// <summary>
    /// Sets audio source based on given DTO.
    /// </summary>
    /// <param name="audioSourceDTO"></param>
    /// <returns></returns>
    public void SetAudioSource(SourceDTO audioSourceDTO)
    {
        AudioSource = audioSourceDTO;
        AudioManager.Instance.SetAudioClip(gameObject, audioSourceDTO);
    }

    #endregion

    #region PRIVATE
    private void Start()
    {
        BackgroundLayers = new List<GameObject>();
        Sources = new List<SourceDTO>();
        SetDefault();
    }

    /// <summary>
    /// Creates new background layer and sets the sorting order in which will be displayed.
    /// </summary>
    /// <param name="sortingOrder"></param>
    /// <returns></returns>
    private GameObject AppendBackgroundLayer()
    {
        GameObject layer = Instantiate(LayerPrefab, transform);
        var spriteRenderer = layer.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1 * BackgroundLayers.Count; //TODO: Fix orientation of camera, so the layer must not be negative.
        BackgroundLayers.Add(layer);

        return layer;
    }

    /// <summary>
    /// Method will decide (based on source valueType) which Layer should be added(Image/Animation).
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="source"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    /// <returns></returns>
    private void SetLayer(GameObject layer, SourceDTO source)
    {
        if (source.Type == SourceType.Image)
        {
            var instance = SpriteManager.Instance;
            if (instance != null)
            {
                instance.SetSprite(layer, source);
            }
        }
        else
        {
            var instance = AnimationsManager.Instance;
            if (instance != null)
            {
                instance.SetAnimation(layer, source);
            }
        }

    }
    #endregion
}
