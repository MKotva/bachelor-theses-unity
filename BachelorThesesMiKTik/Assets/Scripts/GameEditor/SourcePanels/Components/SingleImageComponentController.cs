using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AssetLoaders;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using UnityEngine;

public class SingleImageComponentController : MonoBehaviour, ICreatorComponent
{
    [SerializeField] GameObject SourcePanel;

    private SourcePanelController sourcePanelController;

    private void Start()
    {
        sourcePanelController = GetComponent<SourcePanelController>();
    }

    public void SetObject(GameObject ob)
    {
        var sourceDTO = sourcePanelController.GetData();

        switch(sourceDTO.Type) 
        {
            case SourceType.Image:
                SpriteLoader.SetSprite(ob, sourceDTO.URL, 30, 30); //TODO: Change values to some real stuff, this is just placeholder.
                break;
            case SourceType.Animation:
                AnimationLoader.SetAnimation(ob, ( (AnimationSourceDTO) sourceDTO ).AnimationData, 30, 30); //TODO: Change values to some real stuff, this is just placeholder.
                break;
            case SourceType.Video:
                //TODO: Video loading
                break;
        }

        //TODO: Exception handle
    }
}
