using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SourcePanelController : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown DropDown;
    [SerializeField] public GameObject AnimationButton;
    [SerializeField] public GameObject SourceField;
    [SerializeField] public GameObject AnimationCreator;


    public List<AnimationFrameDTO> Data { get; private set; }

    private GameObject Canvas;
    private GameObject panelInstantiate;
    private AnimationCreatorPopUpController controller;
    private bool isShowing;


    void Awake()
    {
        Data = new List<AnimationFrameDTO>();
        DropDown.onValueChanged.AddListener(delegate
        {
            ChangeField();
        });

        Canvas = GameObject.Find("PopUpCanvas");
    }

    private void ChangeField()
    {
        if(DropDown.value == 2 && !AnimationButton.active)
        {
            SourceField.SetActive(false);
            AnimationButton.SetActive(true);
        }
        else if(!SourceField.active)
        {
            AnimationButton.SetActive(false);
            SourceField.SetActive(true);
        }
    }

    public void OnEditClick()
    {
        if (isShowing)
        {
            return;
        }

        panelInstantiate = Instantiate(AnimationCreator, Canvas.transform);
        controller = panelInstantiate.GetComponent<AnimationCreatorPopUpController>();
        controller.SetCallback(OnExitClick);

        if (Data.Count > 0)
            controller.SetData(Data);

        isShowing = true;
    }

    public void OnExitClick()
    {
        Data = controller.GetData();
        Destroy(panelInstantiate);
        isShowing = false;
    }

    public SourceDTO GetData()
    {
        switch(DropDown.value)
        {
            case 0: return new SourceDTO(SourceType.Image, SourceField.GetComponentInChildren<TMP_InputField>().text);
            case 1: return new SourceDTO(SourceType.Video, SourceField.GetComponentInChildren<TMP_InputField>().text);
            case 2: return new AnimationSourceDTO(SourceType.Animation, Data);
        }
        return new SourceDTO(SourceType.None, string.Empty);
    }
}
