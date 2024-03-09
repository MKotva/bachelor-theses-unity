using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPopUpController : PopUpController
{
    [SerializeField] GameObject ContentView; //Parent gameobject for source panel instance.
    [SerializeField] GameObject LinePrefab; //Source panel frefab.
    [SerializeField] GameObject AudioLoader;

    private BackgroundController backgroundController;
    private List<GameObject> lines;
    private List<SourceDTO> assetSources;

    #region PUBLIC
    /// <summary>
    /// Adds new source line.
    /// </summary>
    public void OnAddLineClick()
    {
        var instance = Instantiate(LinePrefab, ContentView.transform);
        instance.GetComponent<SourcePanelController>().onDestroy += DestroyPanel;
        lines.Add(instance);
    }

    /// <summary>
    /// Clears all added lines and baground images.
    /// </summary>
    public void OnClearClick()
    {
        assetSources.Clear();
        backgroundController.ClearBackground();
        
        foreach(var line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
    }


    /// <summary>
    /// Event handler for SetBackground button click. Loads data from all added lines and then sets background.
    /// </summary>
    public async void OnSetBackgroundClick()
    {
        if(lines.Count == 0)
        {
            return;
        }

        assetSources.Clear();
        foreach (var line in lines)
        {
            var controller = line.GetComponent<AssetSourcePanelController>();
            assetSources.Add(controller.GetData());
        }

       await backgroundController.SetBackground(assetSources);
    }

    /// <summary>
    /// Event handler for Default button click. Resets bacground to a default state.
    /// </summary>
    public void OnSetDefaultClick()
    {
        backgroundController.SetDefault();
    }

    public void OnSetAudioClick()
    {
        var audioLoader = Instantiate(AudioLoader, gameObject.transform).GetComponent<AudioLoaderController>();
        var previous = BackgroundController.Instance.AudioController.AudioSourceDTO;
        if (previous != null)
            audioLoader.Initialize(previous);

        audioLoader.OnSave += BackgroundController.Instance.SetAudioSource;
    }
    #endregion

    #region PRIVATE
    private void Start()
    {
        backgroundController = BackgroundController.Instance;
        lines = new List<GameObject>();
        assetSources = new List<SourceDTO>();
    }

    private void DestroyPanel(int id)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].GetInstanceID() == id)
            {
                lines.RemoveAt(i);
            }
        }
    }
    #endregion
}
