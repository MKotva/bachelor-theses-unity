using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.Managers;
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
    [SerializeField] TMP_Text OutputConsole;

    public delegate void OnCreateCall(AnimationSourceDTO animation);

    private List<GameObject> lines;
    private List<OnCreateCall> callbacks;
    private ImageAnimator animatior;

    /// <summary>
    /// Adds line to creator table.
    /// </summary>
    public void OnAddClick()
    {
        var line = Instantiate(LinePrefab, ContentView.transform);
        line.GetComponent<SourcePanelController>().onDestroy += DestroyPanel;
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
    public void OnCreateClick()
    {
        if (NameField.text == "")
        {
            OutputConsole.text = "Name is empty!";
            return;
        }

        if (AnimationsManager.Instance.ContainsName(NameField.name))
        {
            OutputConsole.text = "Name is already used!";
            return;
        }

        foreach (var callback in callbacks)
        {
            callback.Invoke(GetData());
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

    /// <summary>
    /// Initializes the Animation Creator based on given animation.
    /// </summary>
    /// <param name="data"></param>
    public void SetData(AnimationSourceDTO data)
    {
        NameField.text = data.Name;

        foreach(var item in data.AnimationData)
        {
            var line = Instantiate(LinePrefab, ContentView.transform);
            line.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text = item.DisplayTime.ToString();
            line.transform.GetChild(2).GetComponentInChildren<TMP_InputField>().text = item.URL;

            lines.Add(line);
        }
    }
    #region PRIVATE

    private void Awake()
    {
        InfoPanelController.Instance.AddOnShowListener("AnimationCreator", ErrorHandler, "");
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

    private void OnDestroy()
    {
        var instance = InfoPanelController.Instance;
        if (instance)
            InfoPanelController.Instance.RemoveListener("AnimationCreator");
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

        return new AnimationSourceDTO(data, NameField.text, SourceType.Animation);
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
            }
        }
    }

    private void ErrorHandler(string error)
    {
        OutputConsole.text = error;
    }

    #endregion
}
