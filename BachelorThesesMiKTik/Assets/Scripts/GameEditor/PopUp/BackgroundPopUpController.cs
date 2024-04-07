using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Audio;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BackgroundPopUpController : PopUpController
{
    [SerializeField] GameObject ContentView; //Parent gameobject for source panel instance.
    [SerializeField] GameObject LinePrefab; //Source panel frefab.
    [SerializeField] GameObject AudioLoader;
    [SerializeField] TMP_Dropdown AudioDropDown;

    private BackgroundController backgroundController;
    private List<AssetPanelController> lineControllers;
    private List<SourceReference> assetSources;

    #region PUBLIC
    /// <summary>
    /// Adds new source line.
    /// </summary>
    public void OnAddLineClick()
    {
        AddLine();
    }

    /// <summary>
    /// Clears all added lines and baground images.
    /// </summary>
    public void OnClearClick()
    {
        assetSources.Clear();
        backgroundController.ClearBackground();

        foreach (var line in lineControllers)
        {
            Destroy(line);
        }
        lineControllers.Clear();
    }


    /// <summary>
    /// Event handler for SetBackground button click. Loads data from all added lines and then sets background.
    /// </summary>
    public void OnSetBackgroundClick()
    {
        assetSources.Clear();
        foreach (var line in lineControllers)
        {
            assetSources.Add(line.GetData());
        }

        backgroundController.SetBackground(assetSources);

        var value = AudioDropDown.options[AudioDropDown.value].text;
        if (value != "Create" || value != "None")
            backgroundController.SetAudioSource(new SourceReference(value, SourceType.Sound));
        
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
        lineControllers = new List<AssetPanelController>();
        assetSources = new List<SourceReference>();

        AudioDropDown.onValueChanged.AddListener(AudioDropDownChange);
        var names = AudioManager.Instance.AudioClips.Keys.ToArray();
        SetDropdown(AudioDropDown, names);

        if (backgroundController.Sources.Count != 0)
        {
            if (backgroundController.Sources[0].Name == null &&
                backgroundController.Sources.Count == 1)
                return;

            foreach (var source in backgroundController.Sources)
            {
                var controller = AddLine();
                controller.SetData(source);
                assetSources.Add(source);
            }
        }
    }

    private AssetPanelController AddLine()
    {
        var controller = Instantiate(LinePrefab, ContentView.transform)
                    .GetComponent<AssetPanelController>();
        controller.GetComponent<SourcePanelController>().onDestroyClick += DestroyPanel;
        lineControllers.Add(controller);
        return controller;
    }

    private void DestroyPanel(int id)
    {
        for (int i = 0; i < lineControllers.Count; i++)
        {
            if (lineControllers[i].gameObject.GetInstanceID() == id)
            {
                Destroy(lineControllers[i].gameObject);
                lineControllers.RemoveAt(i);
            }
        }
    }

    private void SetAudioDropDown(string defaultValue = "")
    {
        var instance = AudioManager.Instance;
        if (instance == null)
            return;

        var names = instance.AudioClips.Keys.ToArray();
        SetDropdown(AudioDropDown, names);

        if (defaultValue != "")
        {
            SetDropdownDefaultValue(AudioDropDown, defaultValue);
        }
    }

    private void SetDropdown(TMP_Dropdown dropdown, string[] foundNames)
    {
        dropdown.options.Clear();

        var names = new List<string> { "None" };
        foreach (var name in foundNames)
            names.Add(name);
        names.Add("Create");
        dropdown.AddOptions(names);
    }

    private void SetDropdownDefaultValue(TMP_Dropdown dropdown, string defaultValue)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == defaultValue)
            {
                dropdown.value = i;
                dropdown.onValueChanged.Invoke(i);
                return;
            }
        }
        dropdown.value = 0;
    }

    private void AudioDropDownChange(int newValue)
    {
        var value = AudioDropDown.options[newValue].text;
        if (value == "Create")
        {
            var controller = Instantiate(AudioLoader, gameObject.transform)
                                .GetComponent<AudioLoaderController>();

            controller.OnSave += AudioCreatorExitHandler;
        }
    }

    private void AudioCreatorExitHandler(SourceReference source)
    {
        var manager = SpriteManager.Instance;
        if (manager != null)
        {
            SetAudioDropDown(source.Name);
        }
    }
    #endregion
}
