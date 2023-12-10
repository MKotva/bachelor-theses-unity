using Assets.Core.GameEditor.DTOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSourcePlaceholderController : MonoBehaviour
{
    [SerializeField] public GameObject AnimationCreator;

    public List<AnimationFrameDTO> Data { get; private set; }

    private GameObject panelInstantiate;
    private GameObject Canvas;
    private AnimationCreatorPopUpController controller;
    private bool isShowing;

    private void Start()
    {
        Data = new List<AnimationFrameDTO>();
        Canvas = GameObject.Find("PopUpCanvas");
    }

    public void OnEditClick()
    {
        if(isShowing)
        {
            return;
        }

        panelInstantiate = Instantiate(AnimationCreator, Canvas.transform);
        controller = panelInstantiate.GetComponent<AnimationCreatorPopUpController>();
        controller.SetCallback(OnExitClick);
        
        if(Data.Count > 0)
            controller.SetData(Data);

        isShowing = true;
    }

    public void OnExitClick()
    {
        Data = controller.GetData();
        Destroy(panelInstantiate);
        isShowing = false;
    }
}