using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPopUpController : PopUpController
{
    [SerializeField] GameObject ContentView; //Parent gameobject for source panel instance.
    [SerializeField] GameObject LinePrefab; //Source panel frefab.
    //[SerializeField] List<string> DefaultSources;

    private BackgroundController backgroundController;
    private Stack<GameObject> lines;
    private List<SourceDTO> assetSources;

    private void Start()
    {
        backgroundController = BackgroundController.Instance;
        lines = new Stack<GameObject>();
        assetSources = new List<SourceDTO>();
    }

    /// <summary>
    /// Adds new source line.
    /// </summary>
    public void OnAddLineClick()
    { 
        lines.Push(Instantiate(LinePrefab,ContentView.transform));
    }

    /// <summary>
    /// Removes added source line.
    /// </summary>
    public void OnRemoveLineClick() 
    {
        if (lines.Count > 0)
        {
            var line = lines.Pop();
            Destroy(line);
        }
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
            var controller = line.GetComponent<SourcePanelController>();
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

}
