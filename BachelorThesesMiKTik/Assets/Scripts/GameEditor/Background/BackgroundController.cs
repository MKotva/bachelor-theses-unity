using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackgroundController : Singleton<BackgroundController>, IObjectController
{
    [SerializeField] GameObject LayerPrefab;
    [SerializeField] public TMP_Dropdown AudioDropDown;
    [SerializeField] List<GameObject> DefaultBackgroundPrefabs;
    [SerializeField] public AudioController AudioController;

    public List<GameObject> BackgroundLayers { get; private set; }
    public List<SourceReference> Sources { get; private set; }
    public SourceReference AudioSource { get; private set; }

    #region PUBLIC
    public void SetBackground(List<SourceReference> sources)
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
        foreach (var layerSource in sources)
        {
            AppendLayer(layerSource);
        }
    }

    /// <summary>
    /// Append new layer to the front of background.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    public void AppendLayer(SourceReference source)
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
    public void SetLayer(SourceReference source, int layerId)
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
        Sources.Add(new SourceReference(null, SourceType.Image));
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
    public void SetAudioSource(SourceReference audioSourceDTO)
    {
        if (audioSourceDTO == null)
            return;

        AudioSource = audioSourceDTO;
        AudioManager.Instance.SetAudioClip(gameObject, audioSourceDTO, false);
    }

    public void Play()
    {
        var names = GetAnimationNames();
        if (names.Count != 0)
            AnimationsManager.Instance.OnPlay(names);
        
        if(AudioSource != null)
            AudioManager.Instance.OnPlay(new List<string> { AudioSource.Name });
    }

    public void Pause()
    {
        var names = GetAnimationNames();
        if (names.Count != 0)
            AnimationsManager.Instance.OnPause(names);

        if (AudioSource != null)
            AudioManager.Instance.OnPause(new List<string> { AudioSource.Name });
    }

    public void Enter() {}

    public void Exit()
    {
        var names = GetAnimationNames();
        if (names.Count != 0)
            AnimationsManager.Instance.OnStop(names);

        if (AudioSource != null)
            AudioManager.Instance.OnStop(new List<string> { AudioSource.Name });
    }

    #endregion

    #region PRIVATE
    private void Start()
    {
        BackgroundLayers = new List<GameObject>();
        Sources = new List<SourceReference>();
        SetDefault();
        EditorController.Instance.AddActiveObject(gameObject.GetInstanceID(), this);
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
    private void SetLayer(GameObject layer, SourceReference source)
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
                instance.SetAnimation(layer, source, true, false);
            }
        }

    }

    private List<string> GetAnimationNames()
    {
        var names = new List<string>();
        foreach (var source in Sources)
        {
            if (source.Type == SourceType.Animation)
            {
                names.Add(source.Name);
            }
        }
        return names;
    }
    #endregion
}
