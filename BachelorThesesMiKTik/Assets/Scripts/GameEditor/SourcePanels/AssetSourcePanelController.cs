using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor;
using TMPro;
using UnityEngine;

public class AssetSourcePanelController : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown SourceT;
    [SerializeField] public GameObject AnimationButton;
    [SerializeField] public GameObject SourceField;
    [SerializeField] public GameObject AnimationCreator;

    public AnimationSourceDTO AnimationData { get; private set; }

    private TMP_InputField sourceFieldText;
    private GameObject Canvas;
    private GameObject panelInstantiate;
    private AnimationCreatorPopUpController controller;
    private bool isShowing;


    /// <summary>
    /// This method returns proper SourceDTO class based on selected type in type dropdown.
    /// </summary>
    /// <returns></returns>
    public SourceDTO GetData()
    {
        switch (SourceT.value)
        {
            case 0: return new SourceDTO(SourceType.Image, sourceFieldText.text);
            case 1: return new SourceDTO(SourceType.Sound, sourceFieldText.text);
            case 2: return AnimationData;
        }
        return new SourceDTO(SourceType.None, string.Empty);
    }

    /// <summary>
    /// This is serves as init method for source panel, hence MonoBehavior does not provide callable
    /// contructor.
    /// </summary>
    /// <param name="source"></param>
    public void SetData(SourceDTO source)
    {
        switch (source.Type)
        {
            case SourceType.Image:
                SourceT.value = 0;
                SourceField.GetComponentInChildren<TMP_InputField>().text = source.URL;
                break;
            case SourceType.Sound:
                //SourceT.value = 1;
                //SourceField.GetComponentInChildren<TMP_InputField>().text = source.URL;
                break;
            case SourceType.Animation:
                SourceT.value = 2;
                AnimationData = (AnimationSourceDTO) source;
                break;
            default:
                InfoPanelController.Instance.ShowMessage("Image/Animation panel setting error!");
                break;
        }
    }

    /// <summary>
    /// This method handles animation button click by creating new animation editor panel if panel is not already
    /// displayed (which should not be possible, but j.i.c.). Also sets panel callback method, which will return
    /// data from controller.
    /// </summary>
    public void OnEditClick()
    {
        //TODO: Consider removing this if (blocking panel should remove possibility of summoning another panel)
        if (isShowing)
        {
            return;
        }

        panelInstantiate = Instantiate(AnimationCreator, Canvas.transform);
        controller = panelInstantiate.GetComponent<AnimationCreatorPopUpController>();
        controller.SetCallback(OnExitClick);
        
        if(AnimationData != null) 
            controller.SetData(AnimationData);

        isShowing = true;
    }

    #region PRIVATE
    private void Awake()
    {
        SourceT.onValueChanged.AddListener(delegate
        {
            ChangeField();
        });

        sourceFieldText = SourceField.GetComponentInChildren<TMP_InputField>();
        Canvas = EditorController.Instance.PopUpCanvas.gameObject;
    }

    /// <summary>
    /// This method serve as handler of dropdown value change of type dropdown (representing actual source type e.g Image,Animation ..).
    /// If animation is selected, then the method will activate animation button( which on click creates animation editor panel)
    /// instead of input field.
    /// </summary>
    private void ChangeField()
    {
        if(SourceT.value == 2 && !AnimationButton.activeSelf)
        {
            SourceField.SetActive(false);
            AnimationButton.SetActive(true);
        }
        else if(!SourceField.activeSelf)
        {
            AnimationButton.SetActive(false);
            SourceField.SetActive(true);
        }
    }

    /// <summary>
    /// This is callback method for Animation editor panel exit. Sets data from editor to local data.
    /// </summary>
    private void OnExitClick(AnimationSourceDTO animation)
    {
        AnimationData = animation;
        Destroy(panelInstantiate);
        isShowing = false;
    }
    #endregion
}
