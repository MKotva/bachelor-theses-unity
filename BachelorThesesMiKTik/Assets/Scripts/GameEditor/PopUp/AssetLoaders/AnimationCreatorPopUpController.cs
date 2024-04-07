using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.OutputControllers;
using Assets.Scripts.GameEditor.SourcePanels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationCreatorPopUpController : PopUpController
{
    [SerializeField] public GameObject LinePrefab;
    [SerializeField] public GameObject ContentView;
    [SerializeField] TMP_InputField NameField;
    [SerializeField] Image Image;
    [SerializeField] OutputController OutputConsole;

    public delegate void OnCreateCall(string name);

    private List<GameObject> lines;
    private List<OnCreateCall> callbacks;
    private ImageAnimator animatior;

    /// <summary>
    /// Adds line to creator table.
    /// </summary>
    public void OnAddClick()
    {
        var line = Instantiate(LinePrefab, ContentView.transform);
        line.GetComponent<SourcePanelController>().onDestroyClick += DestroyPanel;
        lines.Add(line);
    }

    /// <summary>
    /// Destroys all lines in Creator table.
    /// </summary>
    public void OnClearClick()
    {
        foreach (var line in lines)
            Destroy(line);

        lines.Clear();
    }    

    /// <summary>
    /// If the name is not used or empty, invokes all callback methods and gives them 
    /// generated AnimationDTO
    /// </summary>
    public async void OnCreateClick()
    {
        if (NameField.text == "")
        {
            OutputConsole.ShowMessage("Name is empty!");
            return;
        }

        if (lines.Count == 0)
        {
            OutputConsole.ShowMessage("You cant create animation with no frames!");
            return;
        }

        if (AnimationsManager.Instance.ContainsName(NameField.name))
        {
            OutputConsole.ShowMessage("Name is already used!");
            return;
        }
        
        var instance = AnimationsManager.Instance;
        if (instance != null)
        {
            var data = GetData();
            if(await instance.AddAnimation(data))
                InvokeCallBacks(data.Name);
        }
    }

    public async void OnPreviewClick()
    { 
        var animationDTO = GetData();
        if (animationDTO.AnimationData.Count == 0)
            return;
        
        var animation = await AnimationLoader.LoadAnimation(animationDTO);

        if(animation == null)
        {
            return;
        }

        animatior = new ImageAnimator(Image, animation, true);
    }

    /// <summary>
    /// Enlist callback funtion of event Create. This method will hand over
    /// the created animation to this callback function.
    /// </summary>
    /// <param name="callbackFunction"></param>
    public void SetCallback(OnCreateCall callbackFunction)
    {
        callbacks.Add(callbackFunction);
    }

    #region PRIVATE

    private void Awake()
    {
        lines = new List<GameObject>();
        callbacks = new List<OnCreateCall>();
    }

    private void Update()
    {
        if (animatior != null)
        {
            animatior.Animate(Time.deltaTime);
        }
    }

    private void InvokeCallBacks(string name)
    {
        foreach (var item in callbacks)
        {
            item.Invoke(name);
        }
    }

    /// <summary>
    /// Creates AnimationSourceDTO from values in panel items.
    /// </summary>
    /// <returns></returns>
    private AnimationSourceDTO GetData()
    {
        var data = new List<AnimationFrameDTO>();
        foreach (var line in lines)
        {
            var displayTime = line.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            var URL = line.transform.GetChild(2).GetComponent<TMP_InputField>().text;

            if (double.TryParse(displayTime, out var time))
                data.Add(new AnimationFrameDTO(time, URL));
        }

        return new AnimationSourceDTO(data, NameField.text);
    }

    /// <summary>
    /// Handler of destroy panel event. Destroys panel based on given 
    /// instance ID;
    /// </summary>
    /// <param name="id"></param>
    private void DestroyPanel(int id)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].GetInstanceID() == id)
            {
                lines.RemoveAt(i);
                Destroy(lines[i].gameObject);
            }
        }
    }

    #endregion
}
